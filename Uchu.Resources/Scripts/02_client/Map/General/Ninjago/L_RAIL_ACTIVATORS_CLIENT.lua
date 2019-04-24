----------------------------------------
-- client side script on start rails for ninjago rails
-- check the server script for set up info scripts\02_server\Map\General\Ninjago\L_RAIL_ACTIVATORS_SERVER.lua
--
-- created by brandi... 6/14/11
---------------------------------------------

require('02_client/Map/General/Ninjago/L_RAIL_POST_CLIENT')

function CheckActiveState(self)
	baseCheckActiveState(self)
	self:RequestPickTypeUpdate()
end

function onRebuildNotifyState(self,msg)
	if msg.iState == 2 then
		CheckActiveState(self)
	end
end

-- make sure the player can use it
function CheckUseRequirements(self, msg)
	-- if this is currently inUse dont spawn anything
    if self:GetNetworkVar("NetworkNotActive") then 
		msg.bCanUse = false
	end
	return msg
end

-- check the picktype state to turn the interact icon on and off
function onGetPriorityPickListType(self, msg)  	
	local myPriority = 0.8  

    if ( myPriority >= msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority  

        if self:GetVar('NotActive') then
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactive pick type     
        end
    end    

    return msg
end 