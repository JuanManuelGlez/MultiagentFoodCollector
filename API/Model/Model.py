from mesa import Agent, Model
from mesa.space import SingleGrid
from mesa.time import RandomActivation
from mesa.datacollection import DataCollector

import numpy as np
import pandas as pd

import time as tm


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

        # Time management
        self.startTime = tm.time()

        # Create Agents
        for i in range(self.numRecolectors):
            x = np.random.randint(0,self.width)
            y = np.random.randint(0,self.height)
            a = RecolectorAgent(i,self)
            self.schedule.add(a)
            self.grid.place_agent(a,(x,y))

    # Put food in the floor
    def putFood(self):
        for i in range(self.totalFood):
            x = np.random.randint(0,self.width)
            y = np.random.randint(0,self.height)
            self.floor[x][y] += 1

    def checkToPutFood(self):
        # get the difference in seconds between current time and start time
        time_difference = tm.time() - self.startTime

        # check if the time difference is a multiple of 5 (every 5 seconds)
        if time_difference % 5 == 0:
            self.startTime = tm.time()
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
