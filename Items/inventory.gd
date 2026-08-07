extends Node

var activeUpgrades = {"health" : 2, }
var inactiveUpgrades = { } 

func addUpgrade(upgradeName) -> void:
	if (!inactiveUpgrades[upgradeName]) :
		inactiveUpgrades[upgradeName] = 1
	else :
		inactiveUpgrades[upgradeName] += 1
