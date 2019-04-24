function onMissionDialogueOK(self, msg)
	local user = msg.responder
	user:TransferToZone{ zoneID = 1300 } --instance type invalid
end