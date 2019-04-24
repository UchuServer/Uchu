
--------------------------------------------------------------
-- includes
--------------------------------------------------------------
require('c_AvantGardens')





--------------------------------------------------------------
-- Render Ready
--------------------------------------------------------------
function onRenderComponentReady( self, msg )
	
	self:EquipItem{ bImmediate = true }
end




--------------------------------------------------------------
-- object notification
--------------------------------------------------------------
function onNotifyObject( self, msg )
	
	if ( msg.name == "unequip" ) then
	
		self:UnEquipItem{ bImmediate = true }

		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "checkForDone", self )
		
	end
	
end




--------------------------------------------------------------
-- timers
--------------------------------------------------------------
function onTimerDone( self, msg )
	
	if ( msg.name == "checkForDone" ) then
	
		if ( self:IsItemEquipped{}.isequipped == true ) then
		
			GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "checkForDone", self )
		
		else
		
			ShowBricksBreaking( self )
			UnEquip( self )
			
		end
	
	end
	
end




--------------------------------------------------------------
-- if appropriate for this instrument, show the equippable break up into bricks
--------------------------------------------------------------
function ShowBricksBreaking( self )
	
	local LOT = self:GetLOT{}.objtemplate
	
	if ( CONSTANTS["INSTRUMENT_HIDE"][LOT] == false ) then
		return
	end
	
	
	-- show the bricks breaking off
	--local player = GAMEOBJ:GetObjectByID( GAMEOBJ:GetLocalCharID() )
	local player = self:GetItemOwner{}.ownerID
	local playerPos = player:GetPosition{}.pos			
	self:Smash{ position = playerPos }
	
	GAMEOBJ:DeleteObject( self )
end





--------------------------------------------------------------
-- make sure this equippable never shows up on the player again
--------------------------------------------------------------
function UnEquip( self )

	--Note:
	
	-- at first I tried GAMEOBJ:DeleteObject( self )
	-- then after playing an instrument, it was possible to equip several inventory items at the same time in whichever hand the equippable was
	
	-- but if I didn't delete the object
	-- then the equippables might show up on the player later when playing a different instrument or fighting the zombies
		
	-- so I do not delete the equippable, but tell it to forget which player it goes onto, so it will not mysteriously re-equip later
		
	self:ClearItemsOwner{}
			
end









