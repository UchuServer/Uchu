--------------------------------------------------------------
-- client side script on the Blue X's in CP for Mission from NT

-- created by brandi.. 2/10/11
--------------------------------------------------------------

local missionID = 1448

function onScopeChanged(self,msg)
	-- if the player entered ghosting range
    if msg.bEnteredScope then  
	-- get the player
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		if not player:Exists() then 
			-- tell the zone control object to tell the script when the local player is loaded
			self:SendLuaNotificationRequest{requestTarget = GAMEOBJ:GetZoneControlID() , messageName="PlayerReady"}
			return
		end
		-- custom function
		CheckMissions(self,player)
	end
end

-- the zone control object says the player is loaded
function notifyPlayerReady(self,zoneObj,msg)
	-- get the player
	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
	
	if not player:Exists() then return end
	-- custom function to see if the players flag is set
	CheckMissions(self,player)
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

----------------------------------------------
-- decide to hide the X or not
----------------------------------------------
function CheckMissions(self,player)

	-- get the flag id set on the X in happy flower
	local myFlag = self:GetVar("flag")
	
	-- if the player is not on the mission, or has used this X, hid it
	if not (player:GetMissionState{missionID = missionID}.missionState == 2) or player:GetFlag{iFlagID = myFlag}.bFlag == true then 
		self:SetVisible{visible = false, fadeTime = 0.0}
		self:SetVar("CanUse",false)
	-- otherwise dont hide it, and set variable that it can be used
	else
		self:SetVar("CanUse",true)
	end
	-- update the shift icon
	self:RequestPickTypeUpdate()
end

----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
 
        if self:GetVar('CanUse') then
            msg.ePickType = 14  -- Interactive pick type  
        else
            msg.ePickType = -1      
        end
    end  
  
    return msg      
end 

----------------------------------------------
-- messages sent from the client
----------------------------------------------
function onScriptNetworkVarUpdate(self, msg)
	-- parse through the table of network vars that were updated
    for k,v in pairs(msg.tableOfVars) do
		
        -- start the qb smash fx, hopefully this is temporary
        if k == "startEffect" and v then
			self:AttachRenderEffectFromLua{ effectType = 10, --AttachEffectMsg::RENDER_EFFECTS_CYCLIC_COLOR_GLOW,
												fadeStart = 0.5,
												delta_darken = 0.5,
												fadeEnd = 0.1,
												delta_lighten = 0.05,
												--effectTime = v,
												alpha = .75,
												color = { r = 0.0, g = 0.0, b = 1.0, a = 0 },
												bAffectIcons = false } -- to make the qb smash blink happen.
		-- the X was used, hide the shift icon										
		elseif k == "XUsed" then
			self:SetVar("CanUse",false)
			self:RequestPickTypeUpdate()
		end
	end
end


		