
--------------------------------------------------------------
-- includes
--------------------------------------------------------------
require('c_AvantGardens')




--------------------------------------------------------------
-- vars
--------------------------------------------------------------
INSTRUMENTS = {}	-- stores all 4 instruments




--------------------------------------------------------------
-- When objects are loaded via zone notification
--------------------------------------------------------------
function onObjectLoadedInstruments( self, msg )

      
	if ( IsValidInstrument( msg.templateID ) ) then
		
		local nextInstrument = #INSTRUMENTS + 1
        INSTRUMENTS[nextInstrument] = msg.objectID:GetID()
        
	end
		
end





--------------------------------------------------------------
-- checks whether the given LOT is one of the instrument quickbuilds
--------------------------------------------------------------
function IsValidInstrument( LOT )

	return ( LOT == CONSTANTS["INSTRUMENT_LOT_GUITAR"] or
		LOT == CONSTANTS["INSTRUMENT_LOT_BASS"] or
		LOT == CONSTANTS["INSTRUMENT_LOT_KEYBOARD"] or
		LOT == CONSTANTS["INSTRUMENT_LOT_DRUM"] )
	
end





--------------------------------------------------------------
-- player left zone
--------------------------------------------------------------
function onPlayerExitInstruments( self, msg )
	
	for index = 1, #INSTRUMENTS do
	
        local instrument = GAMEOBJ:GetObjectByID(INSTRUMENTS[index])
        instrument:NotifyObject{ ObjIDSender = msg.playerID, name = "playerExit" }
        
	end
	
end



