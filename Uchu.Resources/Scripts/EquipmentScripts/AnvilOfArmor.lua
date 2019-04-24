
require('EquipmentScripts/SpawnPowerupsOnTimerThenDie')


function onStartup(self)
    self:SetVar( "numCycles", 8)
    self:SetVar( "secPerCycle", 25)
    self:SetVar( "delayToFirstCycle", 1.5)
    self:SetVar( "deathDelay", 25)
    self:SetVar( "numberOfPowerups", 4)
    self:SetVar( "LootLOT" , 6431)
    onTemplateStartup(self)
end