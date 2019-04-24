--------------------------------------------------------------
-- Description: Server side script for verifying racing 
-- instancing information. Add this script to a minigame instancer
-- to add drag to interact and double click functionality.
--
-- Updated 9/16/10 mrb... updated proximity distance and onCheckWithinBounds
--------------------------------------------------------------

function onStartup(self)
    -- set up a proximity monitor based on the HF interaction distance
    self:SetProximityRadius{radius = self:GetInteractionDistance().fDistance - 1, name = "Interaction_Distance"}
end

function onMatchGetDataForPlayer(self, msg)
	msg.bOK = false

	--Verify that the activityID the player is requesting matches my activity ID and the player dragged an item
	if msg.playerChoices.droppedItem == nil or self:GetActivityID().activityID ~= msg.activityID then
		return
	end
	
	local modObj = GAMEOBJ:GetObjectByID(msg.playerChoices.droppedItem)
	-- Check the players inventory for the item and fail if not
	if not modObj or msg.player:GetInvItemCount{iObjID=modObj}.itemCount < 1 or modObj:GetLOT().objtemplate ~= 8092 then
		return
	end

	-- Save this out now for the destination map	
	msg.finalValues.droppedItem = modObj
	
	-- Approve of the player selections
	msg.bOK = true
	
	-- Calculate the players point worth
	msg.points = 1

	return msg
end

function onCheckWithinBounds(self, msg)
    --print('onCheckWithinBounds ' .. msg.requestingObj:GetItemOwner().ownerID:GetName().name .. ' --> ' .. msg.requestingObj:GetName().name)
    
    local player = msg.requestingObj:GetItemOwner().ownerID
    local playerID = player:GetID()
                                                                           
    -- loop through the objects in proximity and see if the player requesting the double click is there
    for k,v in ipairs(self:GetProximityObjects{name = "Interaction_Distance"}.objects) do
        if v:GetID() == playerID then                     
            self:SetNetworkVar("bPassedCheck", {playerID, msg.requestingObj:GetID()})
            
            local bCanUse = true
            local preConVar = self:GetVar("CheckPrecondition") or ""
            
            -- If there is no precondition then skip this check
            if ( preConVar ~= "" ) then
                local checkPreMsg = player:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar}
                   
                bCanUse = checkPreMsg.bPass
                
                if not checkPreMsg.bPass then
                    --the user is not ready to enter the race 
                    msg.altMessage = "__DONT_DISPLAY_DIALOG__" --This means do not display the dialog at all
                end                
            end
            
            if ( bCanUse ) then
    
                msg.altMessage = "MINGAME_LOBBY_DOUBLE_CLICK_MESSAGE"
        
            end
            
            break
        end
    end
    
    return msg
end
