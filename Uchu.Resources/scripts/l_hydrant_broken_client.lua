
--------------------------------------------------------------
-- Includes
require('o_mis')
require('c_Zorillo')



--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
CONSTANTS["HYDRANT_BOUNCER_LOT"] = 4008




--------------------------------------------------------------
-- Called when object is added to world
--------------------------------------------------------------
function onStartup(self)

	self:SetVar("bRenderReady", false)
	     
    self:SetVar("waterEffect", CONSTANTS["NO_OBJECT"])
    
end
	
	
	


--------------------------------------------------------------
-- Handled when rendering is ready
--------------------------------------------------------------
function onRenderComponentReady(self, msg) 

    self:SetVar("bRenderReady", true)
    
    AddWaterEffect( self )

end

--------------------------------------------------------------
-- add the water particle effect
--------------------------------------------------------------
function AddWaterEffect( self )

    -- return early if render is not ready
    if (self:GetVar("bRenderReady") == false) then
        return
    end
    
    
    -- return out if we already have an effect
    local myEffect = self:GetVar("waterEffect")
	if ( myEffect ) then
        return
	end
	
	
    -- make a new effect
	self:PlayFXEffect{ name = "water", effectID = 384, effectType = "water" }
    -- save the effect
	self:SetVar( "waterEffect", true )

end



function onNotifyObject( self, msg )

	if ( msg.name == "cleanPlayer" ) then
	
		--print( "client-side broken hydrant got msg from bouncer to clean player" )

		self:FireEventServerSide{ senderID = msg.ObjIDSender, args = "cleanPlayer" }
		
	end
end



--------------------------------------------------------------
-- called when a child object is loaded up, in this case, the bouncer
--------------------------------------------------------------
function onChildLoaded( self,msg )
    
    if ( msg.childID:GetLOT().objtemplate == CONSTANTS["HYDRANT_BOUNCER_LOT"] ) then
    
		--print( "bouncer loaded client-side" )
		
		storeParent( self, msg.childID )	
	end

end














