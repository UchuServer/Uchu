----------------------------------------------------
-- script on beck to show the emote tutorial after accepting his missions to salute him

-- written by brandi 6/11/10
-----------------------------------------------------

function onMissionDialogueOK(self,msg)
	if msg.missionID == 315 and msg.iMissionState == 1 then
		UI:SendMessage( "DisplayTutorial", { {"type","speedchat"}, {"showImmediately", true} } )
	end
end