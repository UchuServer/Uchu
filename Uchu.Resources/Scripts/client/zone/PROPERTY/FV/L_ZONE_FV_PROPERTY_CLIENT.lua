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
				defeatedPropFlag = 99 -- when the player builds the claimmarker defeating the maelstrom on this property
			  }
--GROUPS, set in Happy Flower on objects, make sure these match the server script
local Groups = {
				PlaqueGroup = "PropertyPlaque",
				Guard = "Guard"
			   }
local GUIDPeaceful3D = {}
		GUIDPeaceful3D["Audio-Birds"]	= "{2db8a5cc-851d-4ec3-bdb9-be1b3c8376e1}"
		GUIDPeaceful3D["Audio-Wind"]	= "{17e24329-697a-4890-b7d4-0e59a04aa7f2}"
		GUIDPeaceful3D["2D Ambience"]	= "{afac2aa2-6123-4128-a9ef-1ada8074912d}"
       
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
