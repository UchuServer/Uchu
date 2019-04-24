--------------------------------------------------------------
-- stripped down version of the NPC chat script for the guard in NT
-- created by Brandi... 4/12/11
--------------------------------------------------------------


-- create a table of proximity chat for the guard
local proxText = { "NPC_SLEEPING_KNIGHT_BANTER01", "NPC_SLEEPING_KNIGHT_BANTER02" }

--------------------------------------------------------------
-- set the guard up
--------------------------------------------------------------
function onStartup(self)
	-- Set the proximity for the guard to chat when the player is near
	self:SetProximityRadius{radius = 30, name="chatBubbleProx"}
	-- the guard is not in use
	self:SetVar('isInUse', false)
end

function onScriptNetworkVarUpdate(self, msg)
	
	for varName,varValue in pairs(msg.tableOfVars) do
		-- check to see if we have the correct message and deal with it
		if varName == "asleep" then 
			self:RequestPickTypeUpdate()
		end
	end
end

--------------------------------------------------------------
-- override pick type to be interactive
--------------------------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
        if self:GetVar('isInUse') or not self:GetNetworkVar('asleep') then
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactive pick type   
			
        end
    end  
  
    return msg      
end 

--------------------------------------------------------------
-- player interacts with the sleeping guard
--------------------------------------------------------------
function onClientUse(self,msg)
	-- set the guard to in use
	self:SetVar('isInUse', true) 
	-- start a timer to set the guard back
	GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "talking", self )
	-- make the player look like they are talking to the guard
	msg.user:PlayAnimation{animationID = "talk", bPlayImmediate = true}
	-- guard chat "zzzzzzz"
	self:DisplayChatBubble{wsText = Localize("NPC_SLEEPING_KNIGHT_CLICKED01")}
end

--------------------------------------------------------------
-- handle proximity updates
--------------------------------------------------------------
function onProximityUpdate(self, msg)
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 
	-- if the player entered the guards proximity
	if msg.status == "ENTER" and msg.objId:GetID() == player:GetID() then
		
		-- get a random text
		local num = math.random(1, table.maxn(proxText))
		self:DisplayChatBubble{wsText = Localize(proxText[num])}

	end
end

--------------------------------------------------------------
-- set the guard back to normal
--------------------------------------------------------------
function onTimerDone(self, msg)

	if (msg.name == "talking") then 
		self:SetVar('isInUse', false)

		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID()) 
		player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self} 
		
		self:RequestPickTypeUpdate()
	end
end


