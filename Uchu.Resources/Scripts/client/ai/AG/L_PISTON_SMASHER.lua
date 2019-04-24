function onArrived(self, msg)

	if ( msg.wayPoint == 1 ) then

		self:PlayFXEffect{ name = "objects\\cannonbig\\cannonbig", effectType = "onsmash" }

	end

end