--------------------------------------------------------------
-- Transfers player to last known non instance
--
-- updated mrb... 8/25/10 -- added localization key and proximity to close the messagebox
--------------------------------------------------------------
function onStartup(self)
    self:SetProximityRadius{radius = self:GetInteractionDistance().fDistance, name = "interactionProx"}
end

function onUse(self, msg)
    if not msg.user:Exists() then return end
    
    -- if there is a config data variable for the text on the object use it otherwise use the dragon instance.
    local sText = self:GetVar("transferText") or "DRAGON_EXIT_QUESTION"
    
    -- display confirmation tool tip to player
    msg.user:DisplayMessageBox{   bShow = true, imageID = 1, callbackClient = self, 
                                text = sText, identifier = "Instance_Exit"}
end

function onMessageBoxRespond(self, msg)
    if not msg.sender:Exists() then return end
    
    -- transfer player to last non instance
    if msg.identifier == "Instance_Exit" and msg.iButton == 1 then
        msg.sender:TransferToLastNonInstance{ playerID = msg.sender, bUseLastPosition = true }
    end
    
    -- terminate the interaction so the shift icon will come back.
    msg.sender:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}     
end 

function onProximityUpdate(self, msg)
    if msg.name == "interactionProx" and msg.status == "LEAVE" then
        
        -- display confirmation tool tip to player
        msg.objId:DisplayMessageBox{ bShow = false, imageID = 1, callbackClient = self, 
                                    text = sText, identifier = "Instance_Exit"}
    end
end
