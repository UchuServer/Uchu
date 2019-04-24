

function onRebuildNotifyState( self, msg)

	if ( msg.iState == 4 ) then
		
		local choicebuild = self:GetParentObj().objIDParent

		GAMEOBJ:GetZoneControlID():NotifyObject{ name = "ChoicebuildSmashed", ObjIDSender = choicebuild }
	end

end