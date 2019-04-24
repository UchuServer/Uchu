
function onFireEventServerSide(self, msg)
	--print('notify object now')
	if (msg.args == "achieve" ) then
		--print("ive achieved")
		msg.senderID:UpdateMissionTask{taskType = "complete", value = 488, value2 = 1, target = self}
	end

end
