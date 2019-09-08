

require('o_mis')



function onStartup(self)
 end
 
 
 
 
 
 function onSquirtWithWatergun( self, msg )
 
    --print( "------------------------------------------------" )
    --print( "MAZE TROLL: onSquirtWithWatergun" )
    --print( "------------------------------------------------" )
    
    storeObjectByName( self, "shooter", msg.shooterID )
    FaceShooter( self )
    
    DrinkWater( self )
      
 end





function DrinkWater( self )

    --print( "------------------------------------------------" )
    --print( "MAZE TROLL: DrinkWater" )
    --print( "------------------------------------------------" )

    self:PlayAnimation{ animationID = "drink" }
    
end




function FaceShooter( self )

    --print( "------------------------------------------------" )
    --print( "MAZE TROLL: FaceShooter" )
    --print( "------------------------------------------------" )

    local shooter = getObjectByName( self, "shooter" )
    self:FaceTarget{ target = shooter, keepFacingTarget = false, bInstant = false }
end


