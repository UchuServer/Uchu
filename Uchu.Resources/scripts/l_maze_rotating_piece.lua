require('o_mis')
require('o_Main')



function onStartup(self)

end




function onUse( self, msg )

	--print( "-----------------------------------" )
	--print( "Maze Rotating Piece: onUse" )
	--print( "-----------------------------------" )
	
    RotateByNinetyDegrees( self )
	
end




function RotateByNinetyDegrees( self )

	--print( "----------------------------------------" )
	--print( "Maze Rotating Piece: RotateObject" )
	--print( "----------------------------------------" )
	
	
	local newRot = self:GetPosition{}.pos
	newRot.x = 0.0
	newRot.y = 1.5708
	newRot.z = 0.0
    self:RotateObject{ rotation = newRot }
end

