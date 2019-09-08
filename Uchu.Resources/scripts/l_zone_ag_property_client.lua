----------------------------------------------------------------
-- level specific client script for Property Pushback in AG small property
-- this script requires a base script
-- this script should be in the zone script in the DB

-- updated abeechler - 6/16/11 ... abstracted music cues into local variables
----------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('L_BASE_PROPERTY_CLIENT')

--//////////////////////////////////////////////////////////////////////////////////
-- User Config local variables

-- player flags. These have to be different for each property map. these are set up in the db
local flags = {
				defeatedPropFlag = 71 -- when the player builds the claimmarker defeating the maelstrom on this property
			  }
--GROUPS, set in Happy Flower on objects, make sure these match the server script
local Groups = {
				PlaqueGroup = "PropertyPlaque",
				Guard = "Guard"
			   }
  
local GUIDPeaceful3D = {}
        GUIDPeaceful3D["Audio-Birds"] = "{0bb36c9c-e631-459a-8e55-11d8b186a805}"
        GUIDPeaceful3D["Audio-Wind"] = "{7af26988-1438-497a-88e8-8e423a6f977a}"
        
----------------------------------------------------------------
-- Store music var references
----------------------------------------------------------------
local MaelMusicCue = "AG_Survival_2"
local ClearedMusicCue = "Property_Peaceful"
        
----------------------------------------------------------------
-- leave the functions below alone
----------------------------------------------------------------

----------------------------------------------------------------
-- Startup, Sets up us some variables
----------------------------------------------------------------
function onStartup(self)
    setGameVariables(Groups,flags,ClearedMusicCue,MaelMusicCue)
    self:SetVar("GUIDPeaceful3D", GUIDPeaceful3D)
end

----------------------------------------------------------------
-- called when the server script sends a message saying if the property is rented or not
----------------------------------------------------------------
function onScriptNetworkVarUpdate(self,msg)
	baseScriptNetworkVarUpdate(self,msg)
	-- this section only happens on AG Small property and it handles all the bobtooltips for the tutorial missions.
	local player = GAMEOBJ:GetControlledID()
	-- scroll through all the messages in the table of vars
	for k,v in pairs(msg.tableOfVars) do
	
		if k == "PlayerAction" then
			-- the player entered the property edit mode
			if v == "Enter" then
				-- they havent placed a model before, and they are on the place 4 models mission
				if player:GetFlag{iFlagID = 101}.bFlag == false and player:GetMissionState{missionID = 871}.missionState == 2 then
					player:DisplayTooltip{ bShow = true, id = "PropTutModelOut", strText = Localize("TOOLTIP_PROPERTY_MODEL_EQUIP_A"), iTime = 214748 }
				-- the player is on the second tutorial mission
				elseif player:GetMissionState{missionID = 891}.missionState == 2 then
					-- turn off the put on thinking hat tooltip
					player:DisplayTooltip{ bShow = false, id = "PropTutThinkingHat" }
					-- if the player hasnt picked up a model or hasnt rotated a model, tell them to pick up a model
					if player:GetFlag{iFlagID = 109}.bFlag == false or player:GetFlag{iFlagID = 110}.bFlag == false then
						player:DisplayTooltip{ bShow = true, id = "PropTutPickUpModel", strText = Localize("TOOLTIP_PROPERTY_MODEL_PICK_UP"), iTime = 214748 }
					--if the player has picked up and rotated a model, then start the put a model away tutorial
					elseif player:GetFlag{iFlagID = 109}.bFlag == true or player:GetFlag{iFlagID = 110}.bFlag == true then
						UI:SendMessage( "DisplayTutorial", { {"type","propertyEdit"} , {"subType","putAway"} } )
					end
				end
			-- the player equipped a model from their backpack
			elseif v == "ModelEquipped" then
				-- they havent placed a model before, and they are on the place 4 models mission
				if player:GetFlag{iFlagID = 101}.bFlag == false and player:GetMissionState{missionID = 871}.missionState == 2 then
					-- hide the take a model out tooltip and show press shift tooltip
					player:DisplayTooltip{ bShow = false, id = "PropTutPropTutModelOut"}
					player:DisplayTooltip{ bShow = true, id = "PropTutPropTutPressShift", strText = Localize("TOOLTIP_PROPERTY_MODEL_PLACE_A"), iTime = 214748 }
				end
			-- if the player exited property edit mode
			elseif v == "Exit" then
				-- close any tooltips this tutorial might have opened
				player:DisplayTooltip{ bShow = false, id = "PropTut" }
			end
				
		elseif k == "Tooltip" then
			-- display tooltip for place a model
			if v == "PlaceModel" then
				player:DisplayTooltip{ bShow = true, id = "PropTutPlaceModel", strText = Localize("TOOLTIP_PROPERTY_MODEL_PLACE_A"), iTime = 214748 }
			-- display tooltip for put thinking hat on
			elseif v == "ThinkingHat" then
				player:DisplayTooltip{ bShow = true, id = "PropTutThinkingHat", strText = Localize("TOOLTIP_PROPERTY_THINKING_CAP"), iTime = 214748 }
			-- turn off tooltip for pick up a model
			elseif v == "PickUpModelOff" then
				player:DisplayTooltip{ bShow = false, id = "PropTutPickUpModel" }
			-- turn off tooltip for pick up a model, and start the rotate ui tutorial
			elseif v == "Rotate" then
				player:DisplayTooltip{ bShow = false, id = "PropTutPickUpModel" }
				UI:SendMessage( "DisplayTutorial", { {"type","propertyEdit"} , {"subType","rotate"} } )
			-- turn off tooltip for place a model and start ui tutorial for put away a model
			elseif v == "PutAway" then
				player:DisplayTooltip{ bShow = false, id = "PropTutPlaceModel"}
				UI:SendMessage( "DisplayTutorial", { {"type","propertyEdit"} , {"subType","putAway"} } )
			-- turn off tooltip for two more models
			elseif v == "TwoMoreModelsOff" then
				player:DisplayTooltip{ bShow = false, id = "PropTutTwoMoreModels"}	
			-- turn off tooltip for another model and display tooltip for 2 more models
			elseif v == "TwoMoreModels" then
				player:DisplayTooltip{ bShow = false, id = "PropTutAnotherModel"}
				player:DisplayTooltip{ bShow = true, id = "PropTutTwoMoreModels", strText = Localize("TOOLTIP_PROPERTY_MODEL_PLACE_C"), iTime = 214748 }
			-- turn off tooltip for press shift and display tool tip for place another model
			elseif v == "AnotherModel" then
				player:DisplayTooltip{ bShow = false, id = "PropTutPressShift"}
				player:DisplayTooltip{ bShow = true, id = "PropTutAnotherModel", strText = Localize("TOOLTIP_PROPERTY_MODEL_PLACE_B"), iTime = 214748 }
			end
		end
	end
			
end

----------------------------------------------------------------
-- called when the server script notifies the client script
----------------------------------------------------------------
function onNotifyClientObject(self,msg,newMsg)
	baseNotifyClientObject(self,msg,newMsg)
end

----------------------------------------------------------------
-- called when the map is shut down, used to kill the LUT
----------------------------------------------------------------
function onShutdown(self)
	baseShutdown(self,msg,newMsg)
end

----------------------------------------------------------------
-- called when timers are done
----------------------------------------------------------------
function onTimerDone(self,msg)
	baseTimerDone(self,msg,newMsg)
end

----------------------------------------------------------------
-- enviromental settings for the maelstrom inhabiting the property
----------------------------------------------------------------
function maelstromSkyOn(self)

	local envSettings = LEVEL:GetEnvironmentSettings{}
	self:SetVar("ambient", envSettings.ambient)
	self:SetVar("directionalColor", envSettings.directionalColor)
	self:SetVar("specular", envSettings.specular)
	self:SetVar("hemi", envSettings.hemi)
	self:SetVar("lightPosition", envSettings.lightPosition)
	self:SetVar("fogColor", envSettings.fogColor)
	self:SetVar("minDrawDistances", envSettings.minDrawDistances)
	self:SetVar("maxDrawDistances", envSettings.maxDrawDistances)
	self:SetVar("skyDome", envSettings.skydomeFilename)

	LEVEL:SetSkyDome("mesh/env/vfx_propertySky_SKYBOX.nif")	
	
	LEVEL:ModifyEnvironmentSettings{
										ambient = {r = .243, g = .255, b = .569},
										specular = {r = 1, g = 1, b = 1},
										--hemi = {r = 1, g = 1, b = 1},
										directional = {r = .827, g = .820, b = 1},
										lightPosition = {x = -0.48, y = 0.35, z = .80},
										blendTime = .1,
										fogColor = {r = 0, g = 0, b = 0},
										minDrawDistances = {fogNear = 0, fogFar = 600, 
															postFogSolid = 3200, postFogFade = 3200, 
															staticObjectDistance = 8000, dynamicObjectDistance = 8000},
										maxDrawDistances = {fogNear = 0, fogFar = 600, 
															postFogSolid = 3200, postFogFade = 3200, 
															staticObjectDistance = 8000, dynamicObjectDistance = 8000}
										--skydomeFilename = "mesh/env/vfx_propertySky_SKYBOX.nif",
									}
	
	LEVEL:CLUTEffect( "LUT_blue.dds", 1, 0.0, 1.0, false )

end

----------------------------------------------------------------
-- enviromental settings for the maelstrom are defeated from the property
----------------------------------------------------------------
function maelstromSkyOff(self)

	LEVEL:ModifyEnvironmentSettings{ambient = self:GetVar("ambient"),
									directional = self:GetVar("directionalColor"),
									specular = self:GetVar("specular"),
									hemi = self:GetVar("hemi"),
									lightPosition = self:GetVar("lightPosition"),
									fogColor = self:GetVar("fogColor"),
									maxDrawDistances = self:GetVar("maxDrawDistances"),
									minDrawDistances = self:GetVar("minDrawDistances"),
									skydomeFilename = self:GetVar("skyDome"),
									blendTime = 1.0}
	--LEVEL:CLUTEffect( "LUT_2xsunny.dds", 3, 0.0, 1.0, false )
	-- disable the LUT after a time
	GAMEOBJ:GetTimer():AddTimerWithCancel( 2, "DisableLUT", self )

end
