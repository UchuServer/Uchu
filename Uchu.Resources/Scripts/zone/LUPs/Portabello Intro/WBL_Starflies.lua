onMissionDialogueOK = function(self, msg)

 

    if (msg.bIsComplete == true) then

 

        							local starflies = self:GetObjectsInGroup{ group = "Starflies", ignoreSelf = true, ignoreSpawners = true }.objects

                                                local numStarflies = #starflies

                                                for i = 1, numStarflies do

                                                                starflies[i]:StartPathing()

                                                end
                                --msg.responder:UpdateMissionTask{taskType = "complete", value = 664, value2 = 1, target = self}

                                

                

    end    

 

end