--------------------------------------------------------------
-- Client Script for the console that control the bridge in CP

-- created brandi.. 11/19/10 - need to add checkuserequirements to show why the player cant use the bridge
-- updated brandi... 11/29/10 - added check use requirements for popups of why the player cant interact  NEED NEW ICONS
-- updated abeechler... 1/12/11 - updated icons for the console interact pre and post build
-- updated brandi... 1/18/11 - added play audio tie ins
--------------------------------------------------------------

-- audio GUIs for the bridge
local departSound = "{929dffbc-7d09-4e17-809c-df5cff4fcf50}"
local travelSound = "{ae287503-c154-4160-b03f-9fbfda159355}"
local arrivedSound = "{f7aabe02-5828-4e07-9fb2-fb4cccf34874}"

----------------------------------------------
-- check to see if the player can use the console
----------------------------------------------
function onCheckUseRequirements(self,msg)
	-- custom function to get the bridge that goes with that console
	local bridge = GetBridge(self)
	-- if no bridge is found, return out of the script
	if not bridge then return end
	
	-- see if they are built and if their location matches that of the console or the console has been used
	if bridge:GetRebuildState{}.iState == 0 and not self:GetNetworkVar("InUse") then
		if ( msg.isFromUI ) then
			msg.HasReasonFromScript = true  
			msg.Script_IconID = 4099
			msg.Script_Reason = Localize("CP_BUILD_BRIDGE") 
			msg.Script_Failed_Requirement = true  
		end
		msg.bCanUse = false  
	elseif self:GetNetworkVar("InUse") then
		if ( msg.isFromUI ) then
			msg.HasReasonFromScript = true  
			msg.Script_IconID = 4099
			msg.Script_Reason = Localize("CP_BRIDGE_IS_DOWN") 
			msg.Script_Failed_Requirement = true  
		end
		msg.bCanUse = false 
	end
	return msg
end

----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    local myPriority = 0.8
  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority 
		
		msg.ePickType = 14    -- Interactive pick type     

        return msg  
    end  
end

----------------------------------------------
-- called when network var is updated
----------------------------------------------
function onScriptNetworkVarUpdate(self, msg)
	-- parse through the table of network vars that were updated
    for k,v in pairs(msg.tableOfVars) do
		-- custom function to get the bridge that goes with that console
		local bridge = GetBridge(self)
		-- if no bridge is found, return out of the script
		if not bridge then return end
		
        -- start the qb smash fx
        if k == "startEffect" and v then
			-- make sure teh about to die effect isnt happening
			if not self:GetVar("aboutToSmash") then
				bridge:AttachRenderEffectFromLua{ effectType = 10, --AttachEffectMsg::RENDER_EFFECTS_CYCLIC_COLOR_GLOW,
													fadeStart = 0.5,
													delta_darken = 0.5,
													fadeEnd = 0.1,
													delta_lighten = 0.05,
													--effectTime = v,
													alpha = .75,
													color = { r = 0.0, g = 0.0, b = 1.0, a = 0 },
													bAffectIcons = false } -- to make the qb smash blink happen.
			end
			
		-- start the smash bridge effect
		elseif k == "SmashBridge" and v then
			bridge:DetachRenderEffectFromLua{ effectType = 10 }-- to make the qb smash blink stop.
			bridge:AttachRenderEffectFromLua{ effectType = 10, --AttachEffectMsg::RENDER_EFFECTS_CYCLIC_COLOR_GLOW,
                                            fadeStart = 0.5,
                                            delta_darken = 0.5,
                                            fadeEnd = 0.1,
                                            delta_lighten = 0.05,
                                            effectTime = v,
                                            alpha = 0.5,
                                            color = { r = 1.0, g = 1.0, b = 1.0, a = 1.0 },
                                            bAffectIcons = false } -- to make the qb smash blink happen.
			-- set a var that the bridge is about to smash
			self:SetVar("aboutToSmash",true)
			
		-- after the bridge dies set the vars back to default	
		elseif k == "BridgeDead" then
			self:SetVar("aboutToSmash",false)
			
		-- when the bridge is goign to start moving, play sounds and stop the blue flash effect
		elseif k == "BridgeLeaving" and v == true then
			bridge:PlayNDAudioEmitter{m_NDAudioEventGUID = departSound}
			bridge:PlayNDAudioEmitter{m_NDAudioEventGUID = travelSound}
			bridge:DetachRenderEffectFromLua{ effectType = 10 }-- to make the qb smash blink stop.
			
		-- when the bridge reaches the end of it path, stop travel sound and play arrived sound	
		elseif k == "BridgeLeaving" and v == false then
			bridge:StopNDAudioEmitter{m_NDAudioEventGUID = travelSound}
			bridge:PlayNDAudioEmitter{m_NDAudioEventGUID = arrivedSound}
		end
    end 
end

----------------------------------------------
-- custom function to get the bridge associated with this console
----------------------------------------------
function GetBridge(self,msg)
	-- get the config data set on the asset in hf
	local console = self:GetVar("bridge")
	
	if not console then return end
	
	-- get all the items in the bridge group
	local bridge = self:GetObjectsInGroup{ group = "Bridge", ignoreSpawners = true }.objects
	
	-- error check to see if there are briges
	if bridge then
		-- parse through all bridges spawned
		for k,v in ipairs(bridge) do
			-- see if they are built and if their location matches that of the console
			if v:Exists() and v:GetVar("bridge") == console then
				-- see if they are built and if their location matches that of the console
				return v
			end
		end
	end
end