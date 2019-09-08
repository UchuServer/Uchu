
require('SpawnPowerupsOnTimerThenDie')


function onStartup(self)
    self:SetVar( "numCycles", 6)
    self:SetVar( "secPerCycle", 30)
    self:SetVar( "delayToFirstCycle", 1.5)
    self:SetVar( "deathDelay", 30)
    self:SetVar( "numberOfPowerups", 5)
    self:SetVar( "LootLOT" , 935)
    onTemplateStartup(self)
end
