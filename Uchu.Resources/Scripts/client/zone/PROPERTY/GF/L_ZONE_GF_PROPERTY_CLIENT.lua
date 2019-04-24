----------------------------------------------------------------
-- level specific client script for Property Pushback in AG small property
-- this script requires a base script
-- this script should be in the zone script in the DB
----------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('client/zone/PROPERTY/L_BASE_PROPERTY_CLIENT')

--//////////////////////////////////////////////////////////////////////////////////
-- User Config local variables

-- player flags. These have to be different for each property map. these are set up in the db
local flags = {
				defeatedPropFlag = 98 -- when the player builds the claimmarker defeating the maelstrom on this property
			  }
--GROUPS, set in Happy Flower on objects, make sure these match the server script
local Groups = {
				PlaqueGroup = "PropertyPlaque",
				Guard = "Guard"
			   }  
			   
local GUIDPeaceful3D = {}
		GUIDPeaceful3D["Audio-Birds"]		= "{4cb54b8f-ec5d-4298-aa40-b190859ec59f}"
		GUIDPeaceful3D["Audio-Birds2"]		= "{55bed0ad-92f9-4b5f-adf7-2dd0af22f954}"
		GUIDPeaceful3D["Audio-Wind"]		= "{4d877735-1e91-4035-a764-d93f763c9a9f}"
		GUIDPeaceful3D["Audio-Waterfall"]	= "{b5b79e35-9272-41e7-b65d-d6c686541fa9}"
		GUIDPeaceful3D["2D Ambience"]		= "{ddb9f210-6e12-4e8b-b75d-6dce373a6d57}"
       
----------------------------------------------------------------
-- Statrup, Sets up us some variables
----------------------------------------------------------------
function onStartup(self)
    self:SetVar("GUIDPeaceful3D", GUIDPeaceful3D)
end

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
