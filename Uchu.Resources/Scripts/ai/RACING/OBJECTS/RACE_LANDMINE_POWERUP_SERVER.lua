
function onStartup(self)
	
end


function onVehicleGetWeaponPowerupStats(self, msg)
	msg.iTimesCanCast = 3
	--msg.fTimeSecs = 0.0
	return msg
end
