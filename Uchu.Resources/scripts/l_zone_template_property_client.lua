----------------------------------------------------------------
-- level specific client script for Property Pushback in AG small property
-- this script requires a base script
-- this script should be in the zone script in the DB
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
				PlaqueGroup = "PropertyPlaque"
			   }
  
----------------------------------------------------------------
-- leave the functions below alone
----------------------------------------------------------------

----------------------------------------------------------------
-- called when the server script sends a message saying if the property is rented or not
----------------------------------------------------------------
function onScriptNetworkVarUpdate(self,msg)
	setGameVariables(Groups,flags)
	baseScriptNetworkVarUpdate(self,msg)
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

	LEVEL:SetSkyDome("mesh/env/vfx_propertySky_SKYBOX.nif")	
	
    LEVEL:SetLights(
		true, 0x525066,					--ambient color
		false, 0xFFFFFF,					--directional colorZ:\LWO\4_game\client\res\macros\gfstart.scm
		false, 0xFFFFFF,					--specular color/
		true, 0xFFF5CA,					--upper Hemi  color
		true, { 0.18, 0.93, 0.33 },	--directional direction
		true, 0x322A91,					--fog color

		true,                           --modifying draw distances (all of them)
		100.0, 0.0,						--fog near min/max
		500.0, 200.0,					--fog far min/max
		160.0, 160.0,					--post fog solid min/max
		500.0, 500.0,					--post fog fade min/max
		8000.0, 8000.0,	    			--static object cutoff min/max
		8000.0, 8000.0,	     			--dynamic object cutoff min/max

		true, "mesh/env/vfx_propertySky_SKYBOX.nif",
		1.0						-- blend time
	)
	
	LEVEL:CLUTEffect( "LUT_blue.dds", 1, 0.0, 1.0, false )

end

----------------------------------------------------------------
-- enviromental settings for the maelstrom are defeated from the property
----------------------------------------------------------------
function maelstromSkyOff(self)

	LEVEL:SetSkyDome("mesh/env/env_sky_won_ag_property.nif")
	
    LEVEL:SetLights(
		true, 0x6B9EBF,					--ambient color
		false, 0xFFFFFF,					--directional colorZ:\LWO\4_game\client\res\macros\gfstart.scm
		false, 0xFFFFFF,					--specular color
		true, 0xC9E5FF,					--upper Hemi  color
		true, { 0, -2500, 1500 },		--directional direction
		true, 0xFFF7D8,					--fog color

		true,                           --modifying draw distances (all of them)
		125.0, 150.0,						--fog near min/max
		255.0, 250.0,					--fog far min/max
		100.0, 100.0,					--post fog solid min/max
		100.0, 100.0,					--post fog fade min/max
		1000.0, 1000.0,	    			--static object cutoff min/max
		350.0, 350.0,	     			--dynamic object cutoff min/max

		true, "mesh/env/env_sky_won_ag_property.nif",
		1.0					-- blend time
	)

	LEVEL:CLUTEffect( "LUT_2xsunny.dds", 3, 0.0, 1.0, false )
	-- disable the LUT after a time
	GAMEOBJ:GetTimer():AddTimerWithCancel( 5, "DisableLUT", self )

end
