--------------------------------------------------------------

-- L_NPC_NINJA_CLIENT.lua

-- Client side script for the picnic ninja vendor/mission giver
-- created by abeechler.. 6/21/11
-- updated mrb... 6/27/11

--------------------------------------------------------------
local disappearMission = 1882       	-- Mission to deliver Maelstrom cube to Hu, on successful completion tele away
local disappearAnim = "smokebomb"   	-- Anim to play when the ninja must disappear
local disappearFX = "cast"				-- FX to play
local animStartWait = 1.5        		-- period of time to wait before starting the animation actions
local defaultAnimPause = 2.5        	-- Default period of time to pause between missing animation actions

----------------------------------------------
-- Catch and parse dialogue acceptance messages
----------------------------------------------
function onMissionDialogueOK(self, msg)
	-- If we're not on the right mission do nothing
	if msg.missionID ~= disappearMission then return end
    
    local missionState = msg.iMissionState
    
    if((missionState == 4) or (missionState == 12)) then
		-- make the npc non-interactable
		self:SetVar("isInUse", true)
		self:RequestPickTypeUpdate()
		
		-- get the interactionID for the player
		local intID = msg.responder:GetPlayerInteraction().interaction
		
		-- close the multi-interactaion menu
		UI:SendMessage( "ToggleInteractionChoices", {{"visible", false }, {"interactionID", intID}} )
		
		-- Prepare a timer for post vanish
		GAMEOBJ:GetTimer():AddTimerWithCancel(animStartWait, "VanishStart", self)
	end
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
            msg.ePickType = 14    -- Interactive pick type     
        end
    end  
  
    return msg      
end 

----------------------------------------------
-- Process ninja teleportation events
----------------------------------------------
function Disappear(self)
	-- Get the anim time
	local animTimer = self:GetAnimationTime{animationID = disappearAnim}.time
	
	-- If we have an animation play it
	if animTimer < defaultAnimPause then 
		animTimer = defaultAnimPause
	end
	
	-- Prepare a timer for post vanish
	GAMEOBJ:GetTimer():AddTimerWithCancel(animTimer, "VanishComplete", self)
	
	self:PlayFXEffect{name = "smokebomb", effectType = disappearFX, priority = 4.0}
end

----------------------------------------------------------------
-- Called when timers are done
----------------------------------------------------------------
function onTimerDone(self,msg)
	if msg.name == "VanishStart" then
		-- Tell the client to disable itneraction and teleport the 
		Disappear(self)
	elseif msg.name == "VanishComplete" then
	    GAMEOBJ:DeleteObject(self)
	end	
end
