module('Suspend', package.seeall)

local function makecontinue(coro)
	return function(...) 
		local ret = { coroutine.resume(coro, ...) }
		if not ret[1] then
			error(ret[2])
		elseif ret[2] then
			ret[2](unpack(ret, 3))
		end
	end
end

function continue()
	return makecontinue(coroutine.running())
end

function suspend(fn)
	local coro = continue()
	return coroutine.yield(function() fn(coro) end)
end

function shift(fn)
	suspend(function(cont) 
		return cont(fn(cont))
	end)
end

function proc(fn)
	makecontinue(coroutine.create(fn))()
end
