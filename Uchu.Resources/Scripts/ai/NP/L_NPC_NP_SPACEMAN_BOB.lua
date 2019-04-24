function onMissionDialogueOK(self, msg)
    if msg.missionID == 173 and msg.bIsComplete then
        msg.responder:SetImagination{ imagination = 6 }
		msg.responder:UpdateMissionTask{taskType = "complete", value = 664, value2 = 1, target = self}
    end
end 