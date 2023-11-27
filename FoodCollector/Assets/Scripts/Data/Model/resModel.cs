using System;
using System.Collections.Generic;

[Serializable]
public class ResModel
{
    public List<Agent> Agents;

    public List<Food> Food;

    public List<Storage> Storage;

    public bool isChangedRoles;

    public int step;

    public bool foundDeposit;
}