CONSTANTS = {}
CONSTANTS["TRAP_IS_SET"] = false
local interactRadius = 20

function onStartup(self)

    self:SetProximityRadius{radius = interactRadius}
end

function onRebuildNotifyState(self, msg)

	if (msg.iState == 2) then
        CONSTANTS["TRAP_IS_SET"] = true
    end
    if (CONSTANTS["TRAP_IS_SET"]) then
        print("the trap is set!")
    end
end

function onDie(self, msg)
    
    CONSTANTS["TRAP_IS_SET"] = false
end

function onProximityUpdate(self, msg)

    if (CONSTANTS["TRAP_IS_SET"] == true) then
        print("the trap went off!")
    end
end