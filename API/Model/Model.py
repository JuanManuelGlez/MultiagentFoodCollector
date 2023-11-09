from mesa import Agent, Model
from mesa.space import SingleGrid
from mesa.time import RandomActivation
from mesa.datacollection import DataCollector

import numpy as np
import pandas as pd

import time as tm

from Agents.RecolectorAgent import RecolectorAgent
from Agents.ExplorerAgent import ExplorerAgent


class foodColectionModel(Model):
    def __init__(self, width, height, numRecolectors, numExplorers, totalFood):
        self.numRecolectors = numRecolectors
        self.numExplorers = numExplorers
        self.totalFood = totalFood
        self.width = width
        self.height = height
        self.schedule = RandomActivation(self)
        self.grid = SingleGrid(self.width, self.height, True)

        self.minFood = 2
        self.maxFood = 5
        self.currFood = 0

        self.steps = 0

        # Random seed
        np.random.seed(1234)

        # Create Floor
        self.floor = np.zeros((self.width, self.height))

        # data collector
        self.datacollector = DataCollector(
            model_reporters={"Total Food": self.getGrid},
        )

        id = 0
        # Create Agents
        for i in range(numRecolectors):
            x = np.random.randint(0, self.width)
            y = np.random.randint(0, self.height)
            a = RecolectorAgent(id, self)
            self.schedule.add(a)
            self.grid.place_agent(a, (x, y))
            id += 1

        for i in range(numExplorers):
            x = np.random.randint(0, self.width)
            y = np.random.randint(0, self.height)
            a = ExplorerAgent(id, self)
            self.schedule.add(a)
            self.grid.place_agent(a, (x, y))
            id += 1

    # Put food in the floor
    def putFood(self):
        putted = False
        while (not putted and self.currFood < self.totalFood):
            x = np.random.randint(0, self.width)
            y = np.random.randint(0, self.height)
            if (self.floor[x][y] == 0):
                self.floor[x][y] = 1
                self.currFood += 1
                putted = True

    def checkToPutFood(self):
        if (self.steps % 5 == 0):
            self.putFood()

    # get the grid
    def getGrid(self):
        return self.floor.copy()

    # Steps
    def step(self):
        self.schedule.step()
        self.steps += 1
        self.datacollector.collect(self)
        self.checkToPutFood()
