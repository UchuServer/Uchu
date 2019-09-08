
require('SpawnPowerupsOnTimerThenDie')


function onStartup(self)
    self:SetVar( "numCycles", 6)
    self:SetVar( "secPerCycle", 5)
    self:SetVar( "delayToFirstCycle", 1.5)
    self:SetVar( "deathDelay", 30)
    self:SetVar( "numberOfPowerups",4)
    self:SetVar( "LootLOT" , 11910)
    onTemplateStartup(self)
end
