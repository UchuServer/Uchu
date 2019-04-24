----------------------------------------
-- client side script on rail posts for ninjago rails
-- check the server side script for set up info scripts\02_server\Map\General\Ninjago\L_RAIL_POST_SERVER.lua
--
-- created by brandi... 6/14/11
---------------------------------------------

-- GUIDs for rails, based on rail activator component
local railGUIDs = 	{
						[6] = "{50b96be9-4307-40f0-918a-b2bf36b76432}", -- earth
						[7] = "{3f34aa02-9508-4130-9bd0-3b70da3fe329}",	-- lightning
						[8] = "{2d5fa868-2c78-46b4-9a67-f40c09bb2b9e}",	-- ice
						[9] = "{37bc1725-90dc-40c2-ab00-0dd1f2b31cd0}"	-- fire

					}
					

function onStartup(self)

	-- get the rail activator component, use it to find the GUID for this activator
	local railComp = self:GetComponentTemplateID{iComponent = 104}.iTemplateID
	self:SetVar("myGUID",railGUIDs[railComp])
				
end

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
		CheckActiveState(self)
	end
end

-- the zone control object says the player is loaded
function notifyPlayerReady(self,player,msg)
	-- custom function to see if the the fx should be on
	CheckActiveState(self)
	-- cancel the notification request
	self:SendLuaNotificationCancel{requestTarget=player, messageName="PlayerReady"}
end

function CheckActiveState(self)
	baseCheckActiveState(self)
end

function baseCheckActiveState(self)
	-- not not active, so active and play the fx
	if not self:GetVar("NotActive") then
		self:PlayFXEffect{ name = "active" , effectType = "active" }	
		self:PlayFXEffect{ name = "attract" , effectType = "attract" }
		
		-- play sound fx
		self:PlayNDAudioEmitter{m_NDAudioEventGUID = self:GetVar("myGUID")}	
		
	-- not active, dont player the fx
	else
		self:StopFXEffect{name = "active"}
		self:StopFXEffect{name = "attract"}
		
		-- stop sound fx
		self:StopNDAudioEmitter{m_NDAudioEventGUID = self:GetVar("myGUID")}
	end
end

function onDie(self,msg)

	self:StopNDAudioEmitter{m_NDAudioEventGUID = self:GetVar("myGUID")}
	
end

function onScriptNetworkVarUpdate(self,msg)
	-- parse through the table of network vars that were updated
	for k,v in pairs(msg.tableOfVars) do
		-- not not active, set notactive to false (its active)
		if k == "NetworkNotActive" and not v then
			self:SetVar("NotActive", false)
		-- not active, set notactive to true (its not active)
		elseif k == "NetworkNotActive" and v then
			self:SetVar("NotActive", true)
		end
		-- check the active state again to turn fx on and off
		CheckActiveState(self)
	end
end


