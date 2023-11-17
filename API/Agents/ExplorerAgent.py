from mesa import Agent, Model
from enum import Enum
import math

class ExplorerAgent(Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)
        self.type = 1
        self.isPositioned = False
        self.moveDown = False
        self.moveRight = True
        self.moveLeft = False
        self.goalPosition = ()
        self.endPosition = ()
        self.random.seed(12345)

    def step(self):
        (x, y) = self.pos
        
        # Move until all food is generated and agent not positioned
        if self.model.steps >= 55 and not self.isPositioned and self.model.positionZone < 5:
            if self.model.floor[x][y] == 1:
                if not self.foodIsAdded(x, y):
                    self.model.foodPositions.append((x, y))

            elif self.model.floor[x][y] == -1:
                self.model.foundDeposit = True
                self.model.depositCoord = (x, y)
                print("Deposit found at:", (x, y))

            elif self.pos == self.goalPosition:
                self.isPositioned = True
                self.model.positionZone += 1

            if not self.isPositioned:
                result = self.positionAgents()
                if not result == None:
                    moveX, moveY = result
                    if self.model.grid.is_cell_empty((moveX, moveY)):
                        self.model.grid.move_agent(self, (moveX, moveY))

        # Move until all food is generated and if agents are in position explore food
        elif self.model.steps >= 55 and self.model.positionZone == 5:
            if self.model.floor[x][y] == 1:
                if not self.foodIsAdded(x, y):
                    self.model.foodPositions.append((x, y))

            elif self.model.floor[x][y] == -1:
                self.model.foundDeposit = True
                self.model.depositCoord = (x, y)
                print("Deposit found at:", (x, y))
            self.moveInPattern()
            
    # Search deposit
    def searchDeposit(self):
        # check if the agent found the deposit
        x, y = self.pos
        if self.model.floor[x][y] == -1:
            self.model.foundDeposit = True
            self.model.depositCoord = (x, y)
            print("Deposit found at: ", x, y)

        # check if the agent found food
        elif self.model.floor[x][y] == 1:
            if not self.foodIsAdded(x, y):
                self.model.foodPositions.append((x, y))

        possibleSteps = self.getRandomNeighborhood()

        emptySteps = [
            step for step in possibleSteps if self.model.grid.is_cell_empty(step)]

        if emptySteps:
            new_position = self.random.choice(emptySteps)
            self.model.grid.move_agent(self, new_position)

    # Look for food
    def searchFood(self):
        # check if the agent found the deposit
        x, y = self.pos
        # check if the agent found food
        if self.model.floor[x][y] == 1:
            if not self.foodIsAdded(x, y):
                self.model.foodPositions.append((x, y))

        possibleSteps = self.model.grid.get_neighborhood(
            self.pos, moore=True, include_center=False)

        emptySteps = [
            step for step in possibleSteps if self.model.grid.is_cell_empty(step)]

        # exclude the deposit from the possible steps
        for step in emptySteps:
            if self.model.floor[step[0]][step[1]] == -1:
                emptySteps.remove(step)

        if emptySteps:
            new_position = self.random.choice(emptySteps)
            self.model.grid.move_agent(self, new_position)

    # Checks if the food is already added to the list
    def foodIsAdded(self, x, y):
        for food in self.model.foodPositions:
            if (food[0] == x and food[1] == y):
                return True
        return False

    # Get the positions to cover agents
    def positionAgents(self):
        neighborCells = self.getRandomNeighborhood()
        minIndexDistance = -1
        for i in range(len(self.model.zonesDict)):
            if self.unique_id == i:
                self.goalPosition = self.model.zonesDict[i][0][0], self.model.zonesDict[i][0][1]
                self.endPosition = self.model.zonesDict[i][len(self.model.zonesDict[i]) - 1][0], 0
                distances = []
                for step in neighborCells:
                    newX, newY = step
                    goalX, goalY = self.goalPosition
                    if self.model.grid.is_cell_empty((newX, newY)):
                        distances.append(math.sqrt((newX - goalX)**2 + (newY - goalY)** 2))
                if len(distances) > 0:
                    minIndexDistance = distances.index(min(distances))
        if minIndexDistance != -1:
            return neighborCells[minIndexDistance]
        return None
    
    # Move in snake pattern
    def moveInPattern(self):
        (x, y) = self.pos
        if not self.pos == self.endPosition:
            # Move right until right hand side
            if self.moveRight and y < len(self.model.floor) - 1:
                self.model.grid.move_agent(self, (x, y + 1))
            # Move left
            elif self.moveLeft and y > 0:
                self.model.grid.move_agent(self, (x, y - 1))
            # Move down if it comes from the right side or left side
            elif self.moveDown and (self.moveRight and y == (len(self.model.floor) - 1) or (self.moveLeft) and y == 0):
                if self.moveLeft:
                    self.moveLeft = False
                    self.moveRight = True
                else:
                    self.moveRight = False
                    self.moveLeft = True
                self.model.grid.move_agent(self, (x + 1, y))

            # Check for direction switches
            if (y == len(self.model.floor) - 1 and self.moveRight) or (y == 0 and self.moveLeft):
                self.moveDown = True

    # Helper to get neighborhood cells
    def getRandomNeighborhood(self):
        return self.model.grid.get_neighborhood(self.pos, moore = True, include_center = False)

