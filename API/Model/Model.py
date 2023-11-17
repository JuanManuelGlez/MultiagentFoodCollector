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
        self.random.seed(12345)

        # Create Floor
        self.floor = np.zeros((self.width, self.height))
        
        # Zone partition
        self.zonesDict = {}
        self.partitionZone()
        self.positionZone = 0
        self.closestFoodDict = {}
        
        # Change of roles
        self.changedRoles = False
        self.id = 0

        # data collector
        self.datacollector = DataCollector(
            model_reporters={"Food": self.getGrid,
                             "Agents": self.getAgents},
        )
        
        for i in range(numExplorers):
            x = np.random.randint(0, self.width)
            y = np.random.randint(0, self.height)
            a = ExplorerAgent(self.id, self)
            self.schedule.add(a)
            self.grid.place_agent(a, (x, y))
            self.id += 1

        # Put deposit
        self.initDeposit()

    def getEmptyCoords(self, foodToGenerate):
        emptyCoords = []
        while (len(emptyCoords) <= foodToGenerate):
            x = np.random.randint(0, self.width)
            y = np.random.randint(0, self.height)
            if (self.floor[x][y] == 0):
                emptyCoords.append((x, y))
        return emptyCoords

    # Put food in the floor

    def putFood(self):
        foodToGenerate = np.random.randint(self.minFood, self.maxFood)
        if (self.currFood + foodToGenerate > self.totalFood):
            foodToGenerate = self.totalFood - self.currFood
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
            if (agent.type == 1):
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
        if (len (self.foodPositions)) == 47 and not self.changedRoles:
            self.changedRoles = True
            self.changeRoles()
        self.datacollector.collect(self)
        self.schedule.step()
    
    # Partition in zones the grid and floor
    def partitionZone(self):
        zone_rows = 4
        num_zones = len(self.floor) // zone_rows

        for zone_id in range(num_zones):
            start_row = zone_id * zone_rows
            end_row = min((zone_id + 1) * zone_rows, len(self.floor))
            
            zone_data = []

            for agent, (row, col) in self.grid.coord_iter():
                if start_row <= row < end_row:
                    zone_data.append((row, col))

            self.zonesDict[zone_id] = zone_data
    
    # Change of roles
    def changeRoles(self):
        for agent in self.schedule.agents:
            if isinstance(agent, ExplorerAgent) and len(self.foodPositions) == 47:
                # Remove the existing agent
                x, y = agent.pos
                self.grid.remove_agent(agent)
                self.schedule.remove(agent)
                self.id += 1
                # Place an agent of the new type (CollectorAgent in this case)
                new_agent = CollectorAgent(self.id, self)
                self.schedule.add(new_agent)
                self.grid.place_agent(new_agent, (x, y))
