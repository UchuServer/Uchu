
--------------------------------------------------------------
-- Render Ready
--------------------------------------------------------------
function onRenderComponentReady( self, msg )
	
	self:EquipItem{ bImmediate = true }
	print("sssssssssssssssssssssssssssssssssssssssssssssss"")
end




--------------------------------------------------------------
-- object notification
--------------------------------------------------------------
function onNotifyClientObject( self, msg )
	
	if ( msg.name == "equip" ) then
	
		self:EquipItem{ bImmediate = true }
	print("rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr"")
	--	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "checkForDone", self )
		
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
		
			UnEquip( self )
			
		end
	
	end
	
end



--------------------------------------------------------------
-- make sure this equippable never shows up on the player again
--------------------------------------------------------------
function UnEquip( self )

		
	self:ClearItemsOwner{}
			
end









