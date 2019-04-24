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
				defeatedPropFlag = 122 -- when the player builds the claimmarker defeating the maelstrom on this property
			  }
--GROUPS, set in Happy Flower on objects, make sure these match the server script
local Groups = {
				PlaqueGroup = "PropertyPlaque",
				Guard = "Guard"
			   }
  
local GUIDPeaceful3D = {}
		GUIDPeaceful3D["Audio-Birds"]		= "{091064d4-a80a-4c39-a63b-9f9f431afe84}"
		GUIDPeaceful3D["Audio-Birds2"]		= "{c677d8ad-48e6-4600-ab9b-ca50615eab35}"
		GUIDPeaceful3D["Audio-Wind"]		= "{f5635d3e-f7a9-4f4f-9cc8-89989d45cb81}"
		GUIDPeaceful3D["2D Ambience"]		= "{45421e6a-0127-4c5c-8979-a2042dbe5b38}"
        
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
