from mesa import Agent, Model
from mesa.space import SingleGrid
from mesa.time import RandomActivation
from mesa.datacollection import DataCollector

import numpy as np
import pandas as pd

import time as tm

from API.Agents.RecolectorAgent import RecolectorAgent
from API.Agents.ExplorerAgent import ExplorerAgent


class foodColectionModel(Model):
    def __init__(self,width,height,numRecolectors,numExplorers,totalFood):
        self.numRecolectors = numRecolectors
        self.numExplorers = numExplorers
        self.totalFood = totalFood
        self.width = width
        self.height = height
        self.schedule = RandomActivation(self)
        self.grid = SingleGrid(self.width,self.height,True)

        self.minFood = 2
        self.maxFood = 5
        self.currFood = 0

        self.steps = 0

        # Random seed
        np.random.seed(1234)

        # Create Floor
        self.floor = np.zeros((self.width,self.height))

        # data collector
        self.datacollector = DataCollector(
            model_reporters = {"Total Food": self.getGrid},
        )

        # Create Agents
        for i in range(self.numRecolectors):
            x = np.random.randint(0,self.width)
            y = np.random.randint(0,self.height)
            a = RecolectorAgent(i,self)
            self.schedule.add(a)
            self.grid.place_agent(a,(x,y))

        for i in range(self.numExplorers):
            x = np.random.randint(0,self.width)
            y = np.random.randint(0,self.height)
            a = ExplorerAgent(i,self)
            self.schedule.add(a)
            self.grid.place_agent(a,(x,y))

    # Put food in the floor
    def putFood(self):
        for i in range(self.totalFood):
            x = np.random.randint(0,self.width)
            y = np.random.randint(0,self.height)
            self.floor[x][y] += 1

    def checkToPutFood(self):
        if(self.steps % 5 == 0):
            self.putFood()

    # get the grid
    def getGrid(self):
        return self.grid.copy()
    
    # Steps
    def step(self):
        self.schedule.step()
        self.steps += 1
        self.datacollector.collect(self)
        self.checkToPutFood(self)
