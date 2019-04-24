
function onStartup(self)

end


function onVehicleGetWeaponPowerupStats(self, msg)
	msg.iTimesCanCast = 2
	--msg.fTimeSecs = 30.0
	return msg
end
