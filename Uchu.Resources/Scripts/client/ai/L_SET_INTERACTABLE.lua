function onGetPriorityPickListType(self, msg)
    local myPriority = 0.8
        
    if ( myPriority > msg.fCurrentPickTypePriority ) then

       msg.fCurrentPickTypePriority = myPriority
       msg.ePickType = 14    -- Interactive pick type

    end

    return msg
end 