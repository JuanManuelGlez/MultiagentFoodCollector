from mesa import Agent, Model


class ExplorerAgent(Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)
        self.type = 1

    def step(self):
        if (self.model.foundDeposit == False):
            self.searchDeposit()
        else:
            self.searchFood()

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

        possibleSteps = self.model.grid.get_neighborhood(
            self.pos, moore=True, include_center=False)

        emptySteps = [
            step for step in possibleSteps if self.model.grid.is_cell_empty(step)]

        if emptySteps:
            new_position = self.random.choice(emptySteps)
            self.model.grid.move_agent(self, new_position)

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
