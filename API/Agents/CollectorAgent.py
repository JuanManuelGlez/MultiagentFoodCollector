from mesa import Agent, Model
import math

class CollectorAgent(Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)
        self.type = 2
        self.carryFood = False

    def step(self):
        self.model.steps+=1
        x, y = self.pos
        # Agent has not found deposit
        if not self.model.foundDeposit:
            self.searchDeposit()
        # Agent has found deposit
        elif not self.carryFood and self.model.foundDeposit:
            # If found food, mark that position as 0 and mark Carry as True
            if self.model.floor[x][y] == 1:
                self.foundFoodAtPosition()

            # Not found food move to closest food
            else:
                if len(self.model.foodPositions) > 0:
                    self.moveToClosestFood()
                else:
                    self.moveRandomly()

        # Agent has carrying food and the deposit has been found
        elif self.carryFood and self.model.foundDeposit:
            if self.carryFood and (x, y) == (self.model.depositCoord):
                self.dropFood()
            else:
                self.moveFoodToDeposit()
    
    def searchDeposit(self):
        x, y = self.pos
        # Check if the agent found the deposit
        if self.model.floor[x][y] == -1:
            self.model.foundDeposit = True
            self.model.depositCoord = (x, y)
            print("Deposit found at: ", x, y)

        # Check if the agent found food
        elif self.model.floor[x][y] == 1:
            if not self.foodIsAdded(x, y):
                self.model.foodPositions.append((x, y))

        possibleSteps = self.model.grid.get_neighborhood(
            self.pos, moore=True, include_center=False)

        emptySteps = [
            step for step in possibleSteps if self.model.grid.is_cell_empty(step)]

        if emptySteps:
            new_position = self.random.choice(emptySteps)
            self.model.grid.move_agent(self, new_position)

    # Look the minimum distance and return the closest to that current position
    def lookMinDistance(self, neighborCells, closeFoodX, closeFoodY):
        distances = []
        for step in neighborCells:
            newX, newY = step
            if self.model.grid.is_cell_empty((newX, newY)):
                distances.append(math.sqrt((newX - closeFoodX)**2 + (newY - closeFoodY)** 2))
        
        minIndexDistance = distances.index(min(distances))
        return neighborCells[minIndexDistance]
    
    # Look for a position to go to with the min distance to food deposit
    def moveFoodToDeposit(self):
        (depX, depY) = self.model.depositCoord
        neighborCells = self.model.grid.get_neighborhood(
            self.pos, moore = True, include_center = False)
        
        moveX, moveY = self.lookMinDistance(neighborCells,
                                            depX,
                                            depY)
        if self.model.grid.is_cell_empty((moveX, moveY)):
            self.model.grid.move_agent(self, (moveX, moveY))
    
    # Look for the food coords closest to current position
    def getFoodCoords(self):
        x, y = self.pos
        # Obtain the closest coords from food to self
        # Distances list
        distances = []
        for i in range(len(self.model.foodPositions)):
            newX, newY = self.model.foodPositions[i]
            distances.append(math.sqrt((newX - x)**2 + (newY - y)** 2))
        minIndexDistance = distances.index(min(distances))
        return self.model.foodPositions[minIndexDistance]
    
    # Move to the closest Food and move to that position if valid cell
    def moveToClosestFood(self):
        (closeFoodX, closeFoodY) = self.getFoodCoords()
        
        neighborCells = self.model.grid.get_neighborhood(
            self.pos, moore = True, include_center = False)

        (moveX, moveY) = self.lookMinDistance(neighborCells,
                                            closeFoodX,
                                            closeFoodY)
        if self.model.grid.is_cell_empty((moveX, moveY)):
            self.model.grid.move_agent(self, (moveX, moveY))

    # Found Food at position remove from global list once found
    def foundFoodAtPosition(self):
        x, y = self.pos
        self.model.floor[x][y] = 0
        self.carryFood = True
        for tuple in self.model.foodPositions:
            if tuple == (x, y):
                self.model.foodPositions.remove(tuple)
        self.moveFoodToDeposit()
        
        # Handle logic after dropping food
    def dropFood(self):
        self.model.depositQuantity += 1
        self.carryFood = False
        self.moveRandomly()
            
    # Checks if the food is already added to the list
    def foodIsAdded(self, x, y):
        for food in self.model.foodPositions:
            if (food[0] == x and food[1] == y):
                return True
        return False
    
    # Move randomly
    def moveRandomly(self):
        # Obtain random neighbors to move
        neighborRandom = self.model.grid.get_neighborhood(self.pos, moore = True, include_center = False)
        # Select random neighbors to move
        (newX, newY) = self.random.choice(neighborRandom)
        if self.model.grid.is_cell_empty((newX, newY)):
            self.model.grid.move_agent(self, (newX, newY))
    
