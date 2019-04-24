function onGetPriorityPickListType(self, msg)
    
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

    local myPriority = 0.8
        
    if ( myPriority > msg.fCurrentPickTypePriority ) then

        msg.fCurrentPickTypePriority = myPriority
       
        if (player:GetFlag{iFlagID = 66}.bFlag == true) then
        
            msg.ePickType = 14    -- Interactive pick type
        
        else
        
            msg.ePickType = -1
        
        end

    end

    return msg
end 