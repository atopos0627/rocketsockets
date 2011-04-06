﻿using System;

namespace hotstack
{
    public delegate Action OwinBody(
        Func<ArraySegment<byte>, Action, bool> onNext,
        Action<Exception> onError,
        Action onComplete );
}