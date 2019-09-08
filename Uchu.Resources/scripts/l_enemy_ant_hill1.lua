--------------------------------------------------------------
-- (SERVER SIDE) Ant Hill Spawner
--
--------------------------------------------------------------

require('o_mis')
require('L_NP_SERVER_CONSTANTS')

function onStartup(self) 
    GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["ANT_SPAWN_TIMER"] , "SpawnAnt", self )
end


onTimerDone = function(self, msg)
	
	-- check for the local character
    if (msg.name == "SpawnAnt") then
	local mypos = self:GetPosition().pos 
        RESMGR:LoadObject 
			{ 
				objectTemplate	= CONSTANTS["ANT_LOT"],
				x				= (mypos.x +3),
				y				=  mypos.y,
				z				= (mypos.z +3),
				owner			= self 
			}
        GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["ANT_SPAWN_TIMER"] , "SpawnAnt", self )
    end	
end

--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)

    msg.childID:SetVar("attached_path","combatant1")
    msg.childID:SetVar("attached_path_start",0)
    -- start child on path
	msg.childID:FollowWaypoints()

end

      
