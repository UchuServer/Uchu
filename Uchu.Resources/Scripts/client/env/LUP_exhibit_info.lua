-- ***********************************************************

-- HF config data format
-- showcaseName -> 0:stringName    -- The group name of the showcase this is going to pull the data from
-- ***********************************************************
require('o_mis');

function onRenderComponentReady(self,msg)
	
	self:PlayFXEffect{ name = "plaque_attract" , effectType = "attract" }
	--self:PlayFXEffect{ name = "plaquefx" , effectType = "display" }
end

----------------------------------------------
-- sent when the local player interacts with the
-- object
----------------------------------------------
function onClientUse(self, msg) 
    local player = GAMEOBJ:GetControlledID()
    -- check to see if we are the correct player
    if player:GetID() ~= msg.user:GetID() or self:GetVar('isInUse') then return end
    
    -- tell the Story Box UI element to open and what to display, then turn off the interaction icon
    --UI:SendMessage("pushGameState", {{"state", "Story"}, {"context", {{"visible", true }, {"text", getText(self) }, {"senderID", player}, {"callbackObj", self}}  }} )
    local exhibitObj = GetExhibit(self);
    if (exhibitObj ~= nil) then
        exhibitObj:NotifyObject{name = "LUPExhibitShowObject", ObjIDSender = self}
        toggleActivatorIcon(self, true)
    end
end 

function onTerminateInteraction(self,msg)
    -- player was hit close the UI element and turn on the icon
    UI:SendMessage( "ToggleStoryBox", {{"visible", false }} )   
    toggleActivatorIcon(self)
end

----------------------------------------------
-- sent when the object story box is closed;
-- this can be done by hitting the x, esc or enter
----------------------------------------------
function onMessageBoxRespond(self, msg)
    -- UI element has been closed turn on the icon
    toggleActivatorIcon(self)
end

----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
        if self:GetVar('isInUse') then
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactable pick type
        end
    end  
  
    return msg      
end 

function onStartup(self)
end
----------------------------------------------
-- sent when the requested local player is hit 
-- by something, this is used to close the story box
----------------------------------------------
function notifyOnHit(self, other, msg)
    -- player was hit close the UI element and turn on the icon
    UI:SendMessage( "ToggleStoryBox", {{"visible", false }} )   
    toggleActivatorIcon(self)
end

----------------------------------------------
-- toggles the activator Icon based on bHide, 
-- to toggle it on you dont have to pass bHidew
----------------------------------------------
function toggleActivatorIcon(self, bHide)
    local player = GAMEOBJ:GetControlledID()
    
    if not bHide then -- show the icon, cancel notification, set isInUse to false
        bHide = false
        self:SetVar('isInUse', false)
        self:SendLuaNotificationCancel{requestTarget=player, messageName="OnHit"}
    else -- hide the icon, request notification, set isInUse to true
        self:SetVar('isInUse', true)
        self:SendLuaNotificationRequest{requestTarget=player, messageName="OnHit"}
    end
    
    -- request the interaction update
    self:RequestPickTypeUpdate()
end 

function GetExhibit(self)
    local exhibitObj = getObjectByName(self, "myExhibit");
    if(exhibitObj ~= nil) then
        return exhibitObj;
    else
        local groupName = self:GetVar("exhibitName");
        if(groupName == nil) then
            error("Add config data to this object in Happy Flower. showcaseName  0:stringNameofExhibit");
        end
        local objList = self:GetObjectsInGroup{group = groupName, ignoreSelf = true}.objects;
        if (objList and #objList > 1) then
            exhibitObj = objList[2];
            storeObjectByName(self, "myExhibit", exhibitObj)
            return exhibitObj;
        end
    end
    return nil;
end
