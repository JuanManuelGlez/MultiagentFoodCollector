from mesa import Agent, Model
from mesa.space import SingleGrid
from mesa.time import RandomActivation
from mesa.datacollection import DataCollector

import numpy as np
import pandas as pd

import time as tm

from Agents.CollectorAgent import CollectorAgent
from Agents.ExplorerAgent import ExplorerAgent


class foodColectionModel(Model):
    def __init__(self, width, height, numRecolectors, numExplorers, totalFood):
        self.numRecolectors = numRecolectors
        self.numExplorers = numExplorers
        self.totalFood = totalFood
        self.width = width
        self.height = height
        self.schedule = RandomActivation(self)
        self.grid = SingleGrid(self.width, self.height, torus=False)

        # Food parameters
        self.minFood = 2
        self.maxFood = 5
        self.currFood = 0

        # Deposit parameters
        self.foundDeposit = False
        self.depositCoord = None
        self.depositQuantity = 0

        # Food positions
        self.foodPositions = []

        # Steps
        self.steps = 0

        # Random seed
        np.random.seed(1234)

        # Create Floor
        self.floor = np.zeros((self.width, self.height))

        # data collector
        self.datacollector = DataCollector(
            model_reporters={"Food": self.getGrid, 
                             "Agents": self.getAgents},
        )

        id = 0
        # Create Agents
        for i in range(numRecolectors):
            x = np.random.randint(0, self.width)
            y = np.random.randint(0, self.height)
            a = CollectorAgent(id, self)
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

        # Put deposit
        self.initDeposit()

    

    def getEmptyCoords(self, foodToGenerate):
        emptyCoords = []
        while(len(emptyCoords) <= foodToGenerate):
            x = np.random.randint(0, self.width)
            y = np.random.randint(0, self.height)
            if(self.floor[x][y] == 0):
                emptyCoords.append((x, y))
        return emptyCoords


    # Put food in the floor
    def putFood(self):
        foodToGenerate = np.random.randint(self.minFood, self.maxFood)
        if(self.currFood + foodToGenerate < self.totalFood):
            emptyCoords = self.getEmptyCoords(foodToGenerate)
            for coord in emptyCoords:
                self.floor[coord[0]][coord[1]] = 1
                self.currFood += 1

    def checkToPutFood(self):
        if ((self.steps + 1) % 5 == 0):
            self.putFood()

    # get the grid
    def getGrid(self):
        return self.floor.copy()
    
    # get the agents
    def getAgents(self):
        agentsPosition = np.zeros((self.width, self.height))
        for agent in self.schedule.agents:
            x, y = agent.pos
            if(agent.type == 1):
                agentsPosition[x][y] = 1
            else:
                agentsPosition[x][y] = 2
        return agentsPosition
    
    def initDeposit(self):
        x = np.random.randint(0, self.width)
        y = np.random.randint(0, self.height)
        self.floor[x][y] = -1

    # Steps
    def step(self):
        self.steps += 1
        self.checkToPutFood()
        self.datacollector.collect(self)
        self.schedule.step()
