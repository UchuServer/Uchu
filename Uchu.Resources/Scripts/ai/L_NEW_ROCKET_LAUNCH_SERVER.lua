require('o_mis')

function onStartup(self)
--    print("New Rocket Launch Server")
	self:SetVar("nPlayers",0)
	self:SetVar("currentPlayer",nil)
end

function onFireEventServerSide(self, msg)
    local targetZone = self:GetVar("targetZone")
    local targetScene = self:GetVar("targetScene")
    if targetScene == nil then targetScene = "" end
    local gmlevel = self:GetVar("GMLevel")
    if gmlevel == nil then gmlevel = 0 end

--    print ("FireServerEvent " .. msg.args .. " target " .. targetZone .. ":" .. targetScene .. " " .. gmlevel .. "sender " .. msg.senderID:GetID() )

    if msg.args == "ClearLauncher" then
        GAMEOBJ:GetTimer():CancelTimer("LaunchTimedOut",self)
        self:SetVar("nPlayers",0)
        self:SetVar("currentPlayer",nil)
    end

    if (msg.args == "ZonePlayer") then
        if msg.senderID:GetGMLevel{}.ucGMLevel >= gmlevel then
            msg.senderID:TransferToZone{ zoneID = self:GetVar("targetZone"), spawnPoint = self:GetVar("targetScene"), bCheckTransferAllowed = true }
        end
    end
end


function onCheckUseRequirements(self,msg)
--    print ("CheckUseRequirements for the Rocket Launch")

    local player = msg.objIDUser
    local item = msg.objIDObjectForUse
    local itemlot = msg.itemLOT

    msg.bCanUse = true

    local nPlayers = self:GetVar("nPlayers")
    if nPlayers>0 then
        local player=getObjectByName(self,"currentPlayer")
        if player ~= nil and player:Exists() then
            msg.bCanUse = false
            return msg
        else
            nPlayers = 0
        	self:SetVar("currentPlayer",nil)
        end
    end
    
    local assemblyComponent = 61
    local rocketAssemblyID = 1

    local itemid = player:GetComponentData  { objLOT=itemlot, componentType=assemblyComponent }.componentID
    if itemid ~= rocketAssemblyID then
        player:DisplayTooltip { bShow = true, strText = "You can only drop rockets on the launchpad. Drop your rocket from your Model tab.", iTime = 5000 }
        msg.bCanUse=false
    end
    
    return msg
end


function onUseItemOn(self,msg)
    print ("ObjUseItemOn for the Rocket Launch")
    local player = msg.serverPlayerID
    local rocket = msg.itemToUse

--    print ("NRLS player " .. player:GetID() .. " rocket " .. rocket:GetID() )
    
    self:SetVar("nPlayers",1)
    storeObjectByName(self,"currentPlayer",player)
    storeObjectByName(self,"rocket",rocket)

    self:FireEventClientSide{senderID = player, args = "RocketEquipped" , object = rocket }
    
  	GAMEOBJ:GetTimer():AddTimerWithCancel( 90, "LaunchTimedOut",self )

end

function onCheckWithinBounds(self,msg)
--    print ("BoundsCheck for the Rocket Launch")

    local nPlayers = self:GetVar("nPlayers")
    if nPlayers>0 then
        local player=getObjectByName(self,"currentPlayer")
        if player ~= nil and player:Exists() then
            msg.bInBounds = false
            return msg
        else
            nPlayers = 0
        	self:SetVar("currentPlayer",nil)
        end
    end

    local rocket = msg.requestingObj
    local player = rocket:GetItemOwner().ownerID
    
--    print ("Rocket " .. rocket:GetID() .. " player " .. player:GetID() )
    
    local selfpos = self:GetPosition{}.pos
    local playerpos = msg.point
    
    local distance2 = (selfpos.x-playerpos.x)*(selfpos.x-playerpos.x) + (selfpos.z-playerpos.z)*(selfpos.z-playerpos.z)
    if distance2 > 100 then
        msg.bInBounds = false
        return msg
    end 

    self:SetVar("nPlayers",1)

    msg.bInBounds = true
    storeObjectByName(self,"currentPlayer",player)
    self:FireEventClientSide{senderID = player, args = "RocketEquipped" , object = rocket }
  	GAMEOBJ:GetTimer():AddTimerWithCancel( 90, "LaunchTimedOut",self )
    return msg

end

-- the LaunchTimedOut is a hack.  If, for whatever reason, the launch
-- sequence doesn't complete as it ought, this will clear the server
-- script and allow the next person to launch
function onTimerDone (self,msg)
    if (msg.name == "LaunchTimedOut") then
        self:SetVar("nPlayers",0)
        self:SetVar("currentPlayer",nil)
    end
end
