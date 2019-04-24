local ProxRadius = 6

CONSTANTS = {}
CONSTANTS["ZERO"] = {x = 0, y = 0, z = 0}

function onStartup(self)
self:SetVar("ExitPoint", CONSTANTS["ZERO"])
self:SetVar("ExitPointExist", 0)
self:SetProximityRadius{radius = ProxRadius}
end

function onProximityUpdate(self, msg)
print("asdfasdfasgadfhsdghsfgjsdfgsdhaglkjasdfhg")
    local b = self:GetVar("ExitPointExist")
    if msg.status == "ENTER" and b == 1 then
        local target = msg.objId
        local faction = target:GetFaction()       
	local vec = self:GetVar("ExitPoint")
	print("Portal " .. vec.x .. " " .. vec.y .. " " .. vec.z)
        target:Teleport{pos = vec}
    end -- end if msg.status == "ENTER"
end 


function onSetPortalExit(self, msg)
	local target = msg.objectID
	self:SetVar("ExitPoint", msg.ExitPortal)
	self:SetVar("ExitPointExist", 1)
end
