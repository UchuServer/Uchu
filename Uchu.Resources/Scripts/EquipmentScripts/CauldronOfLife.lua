
require('EquipmentScripts/SpawnPowerupsOnTimerThenDie')


function onStartup(self)
    self:SetVar( "numCycles", 10)
    self:SetVar( "secPerCycle", 20)
    self:SetVar( "delayToFirstCycle", 1.5)
    self:SetVar( "deathDelay", 20)
    self:SetVar( "numberOfPowerups", 3)
    self:SetVar( "LootLOT" , 177)
    onTemplateStartup(self)
end