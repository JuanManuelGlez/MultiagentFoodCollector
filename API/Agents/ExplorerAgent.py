from mesa import Agent, Model


class ExplorerAgent(Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)
        self.type = 1
