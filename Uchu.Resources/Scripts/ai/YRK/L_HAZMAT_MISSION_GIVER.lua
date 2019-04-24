require('State')
require('o_mis')

CONSTANTS = {}
CONSTANTS["FACTION"] = 5



function onStartup(self)
	
	self:SetFaction { faction = CONSTANTS["FACTION"] }
	
end





function onSquirtWithWatergun( self, msg )

    --print( "------------------------------------------------" )
    --print( "SQUIRT HAZMAT MISSION GIVER WITH WATERGUN" )
    --print( "------------------------------------------------" )

    --nothing to do here because the destink cast already takes care of it
		
end