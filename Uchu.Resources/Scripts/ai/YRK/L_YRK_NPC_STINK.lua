--------------------------------------------------------------
-- (SERVER SIDE) Script for stink cloud NPCs
--
--------------------------------------------------------------
require('o_mis')

--------------------------------------------------------------
-- Locals and Constants
--------------------------------------------------------------
local CONSTANTS = {}
CONSTANTS["DestinkDuration"] = 4.0
CONSTANTS["RolloverDuration"] = 0.2
CONSTANTS["BROOMBOT_LOT"] = 3247
CONSTANTS["bubbleradius"] = 2.0

function onStartup(self)
	self:SetProximityRadius{ radius = CONSTANTS["bubbleradius"]}
end

function onProximityUpdate(self, msg)
    -- if it's a player
	if msg.status == "ENTER" and msg.objId:GetFaction().faction == 1 then
	    -- check for player in bubble, clean stink cloud as a result
		local bPlayerInBubble = msg.objId:IsMiniFigInABubble().bBubble
		if ( bPlayerInBubble ) then
			local currencyMsg = self:RollCurrency{iTable = 1, iLevel = 1}
			self:DropLoot{iCurrency = currencyMsg.iCurrency, owner = msg.objId, rerouteID = msg.objId, sourceObj = self}
			self:Die()
			GAMEOBJ:GetZoneControlID():NotifyObject{ name="stink_cloud_cleaned_by_player", ObjIDSender = msg.objId, param1 = self:GetVar("StinkCloudNum") }
		end
	end
end

--------------------------------------------------------------
-- Called when this object is destinked
--------------------------------------------------------------
function onSquirtWithWatergun(self, msg)

    -- if the shooter is a broom bot
    if (msg.shooterID:GetLOT().objtemplate == CONSTANTS["BROOMBOT_LOT"]) then

        -- delay to match animation
        GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["DestinkDuration"] , "RemoveSelf", self )
    
        -- @TODO: forward player owned broombot info

        -- see if this broombot was built by a player
        local player = getObjectByName( msg.shooterID, "playerBuilder" )
        if (player) then
            -- Tell the zone script a stink cloud was cleaned. Pass who cleaned it and the skunk number
            GAMEOBJ:GetZoneControlID():NotifyObject{ name="stink_cloud_cleaned_by_player", ObjIDSender = player, param1 = self:GetVar("StinkCloudNum") }
        else
            -- Tell the zone script a stink cloud was cleaned. Pass who cleaned it and the skunk number
            GAMEOBJ:GetZoneControlID():NotifyObject{ name="stink_cloud_cleaned_by_broombot", ObjIDSender = msg.shooterID, param1 = self:GetVar("StinkCloudNum") }
        end
        
    
    else

        -- @TODO: update player task for missions
          --this is hacky and not the right way we should be doing this, but it works, non the less
	-- Using the "Kill" mission type in the DB, we are able to update the mission through the objects by calling
	--the "kill" tasktype with calues of one.  The downfall is that this will only allow one mission on the designated objects that
	--use the kill type.  The commented out updatemissiontask functions are the way we should be doing this but is broken.
			msg.shooterID:UpdateMissionTask {target = self, value = 1, value2 = 1, taskType = "kill"}
         --msg.shooterID:UpdateMissionTask {target = self, value = 166, value2 = 1, taskType = "complete"}
        -- msg.shooterID:UpdateMissionTask {target = self, value = 167, value2 = 1, taskType = "complete"}
        --msg.shooterID:UpdateMissionTask {target = self, value = 168, value2 = 1, taskType = "complete"}
         local currencyMsg = self:RollCurrency{iTable = 1, iLevel = 1}
         self:DropLoot{iCurrency = currencyMsg.iCurrency, owner = msg.shooterID, rerouteID = msg.shooterID, sourceObj = self}
        -- Tell the zone script a stink cloud was cleaned. Pass who cleaned it and the skunk number
        GAMEOBJ:GetZoneControlID():NotifyObject{ name="stink_cloud_cleaned_by_player", ObjIDSender = msg.shooterID, param1 = self:GetVar("StinkCloudNum") }
        
    
        -- else this is the player so kill it instantly
        self:Die()
        
    end

end


--------------------------------------------------------------
-- called when timer complete
--------------------------------------------------------------
function onTimerDone(self, msg)

    -- cleaned by a broombot
    if (msg.name == "RemoveSelf") then
        self:Die()
    end

end
