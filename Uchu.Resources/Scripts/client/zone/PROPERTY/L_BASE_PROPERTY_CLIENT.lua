----------------------------------------------------------------
--Base Client script for property pushback. 
--this script is required from a level specific script
-- this script will only work with the server script as well

-- updated abeechler - 6/16/11 ... abstracted music cues into local variables
----------------------------------------------------------------

----------------------------------------------------------------
-- Store music var references
----------------------------------------------------------------
local ClearedMusicCue = "Property_Peaceful"
local MaelMusicCue = "Property_Maelstrom"

----------------------------------------------------------------
-- Define empty tables that will be set from the level specific script
----------------------------------------------------------------
local Group = {}
local Flags = {}

local GUIDMaelstrom3D = {}
        GUIDMaelstrom3D["Audio-Birds"] = "{333e951e-c0af-4e16-955a-c5348b948b4a}"
        GUIDMaelstrom3D["Audio-Wind"] = "{4f9cc3d8-01a9-41c4-a078-a238cf1bc99b}"
----------------------------------------------------------------
-- variables passed of the level specific script that are used throughout the base script
----------------------------------------------------------------
function setGameVariables(passedGroups,passedFlags,clearedMusicCue,maelMusicCue)
	Group = passedGroups
	Flags = passedFlags
	MaelMusicCue = maelMusicCue or MaelMusicCue
	ClearedMusicCue = clearedMusicCue or ClearedMusicCue
end

----------------------------------------------------------------
-- called when the server script sets a network var
----------------------------------------------------------------
function baseScriptNetworkVarUpdate(self,msg)
	
	local player = GAMEOBJ:GetControlledID()

	-- the property is unclaimed, turn the visiblity on the border and the vendor off
	if msg.tableOfVars["unclaimed"] then
		borderOff(self)
		if player:GetFlag{ iFlagID = Flags.defeatedPropFlag }.bFlag == false then
			vendorOff(self)
		end
		if not self:GetVar("maelstromMusicPlayed") then
            --Peaceful sounds will change from map to map, so a child script will have to set the sounds as a var
            if(self:GetVar("GUIDPeaceful3D")) then
                handle3DSounds(self, self:GetVar("GUIDPeaceful3D"), true)
            end
            SOUND:ActivateNDAudioMusicCue(ClearedMusicCue)
		end
	-- the property is rented
	elseif msg.tableOfVars["renter"] then
		-- if the local player is not the renter, turn the border off
		if msg.tableOfVars["renter"] ~= player:GetID() then
			borderOff(self)
		end
        --Peaceful sounds will change from map to map, so a child script will have to set the sounds as a var
		if(self:GetVar("GUIDPeaceful3D")) then
		    handle3DSounds(self, self:GetVar("GUIDPeaceful3D"), true)
		end
		SOUND:ActivateNDAudioMusicCue(ClearedMusicCue)
	end
end

----------------------------------------------------------------
-- turns the visiblity on the property off
----------------------------------------------------------------
function borderOff(self)
	-- get the property plaque by group set in happy flower, the property plaque is coded to share certain information with LUA
	local propertyPlaques = self:GetObjectsInGroup{ group = Group.PlaqueGroup, ignoreSpawners = true }.objects
	-- make sure it got something from the group
	if propertyPlaques then
		for i = 1, table.maxn(propertyPlaques) do
			-- use the property plaque to turn the visiblility of the border asset off
			propertyPlaques[i]:SetPropertyBoundsVisibility{visible = false}
				
		end
	else
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "checkBorderAgain", self ) 
	end
end

----------------------------------------------------------------
-- turns the visiblity on the property off
----------------------------------------------------------------
function vendorOff(self)
	-- get the property plaque by group set in happy flower, the property plaque is coded to share certain information with LUA
	local propertyPlaques = self:GetObjectsInGroup{ group = Group.PlaqueGroup, ignoreSpawners = true }.objects
	-- make sure it got something from the group
	if propertyPlaques then
		for i = 1, table.maxn(propertyPlaques) do
			-- use the property plaque to turn the visiblility of the vendor asset off
			propertyPlaques[i]:SetPropertyVendorVisibility{visible = false}
				
		end
	else
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "checkVendorAgain", self ) 
	end
end

----------------------------------------------------------------
-- called when the script is sent messages from the server script
----------------------------------------------------------------
function baseNotifyClientObject(self,msg)
	-- turn the normal property sky on
	if msg.name == "SkyOff" then
		maelstromSkyOff(self)
		
        --Peaceful sounds will change from map to map, so a child script will have to set the sounds as a var
		if(self:GetVar("GUIDPeaceful3D")) then
		    handle3DSounds(self, self:GetVar("GUIDPeaceful3D"), true)
		end
	    SOUND:ActivateNDAudioMusicCue(ClearedMusicCue)
	-- turn the maelstrom sky on
	elseif msg.name == "maelstromSkyOn" then
		maelstromSkyOn(self)
		
		handle3DSounds(self, GUIDMaelstrom3D, true)
	    SOUND:ActivateNDAudioMusicCue(MaelMusicCue)
	    self:SetVar("maelstromMusicPlayed", true)
	else
	-- get the property plaque by group set in happy flower, the property plaque is coded to share certain information with LUA
	local propertyPlaques = self:GetObjectsInGroup{ group = Group.PlaqueGroup, ignoreSpawners = true }.objects

	-- turn the visiblity on the vendor asset on
	if msg.name == "vendorOn" then
		if propertyPlaques then
			for i = 1, table.maxn(propertyPlaques) do
				propertyPlaques[i]:SetPropertyVendorVisibility{visible = true}
			end
		end
	

	-- turn the property border on
	elseif msg.name == "boundsOn" then
		if propertyPlaques then
			for i = 1, table.maxn(propertyPlaques) do
				propertyPlaques[i]:SetPropertyBoundsVisibility{visible = true, fadeTime=0.5}
			end
		end
	-- play the turn on animation for the border
	elseif msg.name == "boundsAnim" then

		if propertyPlaques then
			for i = 1, table.maxn(propertyPlaques) do
				propertyPlaques[i]:SetPropertyBoundsVisibility{visible = true, animationName = "BorderIn"}
			end
		end
	elseif msg.name == "GuardChat" then
	    local Guard = self:GetObjectsInGroup{ group = Group.Guard, ignoreSpawners = true }.objects[1]
		Guard:DisplayChatBubble{wsText = Localize("PROPERTY_GUARD") } --"I need to go help other minifigures secure their properties. Have fun."
	elseif msg.name == "PlayCinematic" then
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		UI:SendMessage( "pushGameState", {{"state", "cinematic" }} )
		player:PlayCinematic { pathName = msg.paramStr } 
		local cineTime = tonumber(LEVEL:GetCinematicInfo(msg.paramStr)) or 1
		GAMEOBJ:GetTimer():AddTimerWithCancel( cineTime, "cinematicTimer",self )
			
			if msg.paramStr == "DestroyMaelstrom" then
			    --turn off the maelstrom ambients
		        handle3DSounds(self, GUIDMaelstrom3D, false)
	            SOUND:DeactivateNDAudioMusicCue(MaelMusicCue)
		        SOUND:FlashNDAudioMusicCue("Property_Cinematic")
			end
		end
	end
end

function handle3DSounds(self, sounds, startSound)
		--iterate through all our 3D sounds, find the object group that they need to be played on
	for groupName, GUID in pairs(sounds) do
		local emitters = self:GetObjectsInGroup{ group = groupName, ignoreSpawners = true }.objects
		for index, emitter in ipairs(emitters) do
			if(startSound) then
			    emitter:Play3DAmbientSound{m_NDAudioEventGUID = GUID}
			else
			    emitter:Stop3DAmbientSound{m_NDAudioEventGUID = GUID}
			end
		end
	end
end

----------------------------------------------------------------
-- called when the map shuts down
----------------------------------------------------------------
function baseShutdown(self)
    DisableLUT(self)
end

----------------------------------------------------------------
-- set the LUT back to normal
----------------------------------------------------------------
function DisableLUT(self)
	LEVEL:CLUTEffect( "(none)", 0.0, 1.0, 0.0, false )
end

----------------------------------------------------------------
-- called when timers are done
----------------------------------------------------------------
function baseTimerDone(self,msg)
	-- set the LUT back to normal
	if msg.name ==  "DisableLUT" then
		DisableLUT(self)
	elseif msg.name ==  "checkBorderAgain" then
		borderOff(self)
	elseif msg.name ==  "checkVendorAgain" then
		vendorOff(self)
	elseif msg.name ==  "cinematicTimer" then
		UI:SendMessage( "popGameState", {{"state", "cinematic"}} )
	end
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
	
    LEVEL:SetLights(
		true, 0x3e4191,					--ambient color
		false, 0xd3d1ff,					--directional color
		false, 0xFFFFFF,					--specular color/
		true, 0xFFF5CA,					--upper Hemi  color
		true, { -0.83, 0.53, -0.16 },	--directional direction
		true, 0x333333,					--fog color

		true,                           --modifying draw distances (all of them)
		0, 0.0,						--fog near min/max
		150.0, 150.0,					--fog far min/max
		3200.0, 3200.0,					--post fog solid min/max
		100.0, 100.0,					--post fog fade min/max
		8000.0, 8000.0,	    			--static object cutoff min/max
		8000.0, 8000.0,	     			--dynamic object cutoff min/max

		true, "mesh/env/vfx_propertySky_SKYBOX.nif",
		0.5						-- blend time
	)
	
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