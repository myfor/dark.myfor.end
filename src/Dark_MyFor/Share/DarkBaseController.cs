﻿using Domain;
using Microsoft.AspNetCore.Mvc;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Dark_MyFor.Share
{
    /// <summary>
    /// 这是一个基类 API 控制器, 所有 API 控制器都必须直接或间接继承此控制器
    /// </summary>
    [ApiController]
    public abstract class DarkBaseController : ControllerBase
    {
        /// <summary>
        /// 打包响应数据
        /// </summary>
        /// <returns></returns>
        protected ActionResult Pack(Resp resp)
        {
            ActionResult result = resp.StatusCode switch
            {
                Resp.Code.Success => Ok(resp),
                Resp.Code.Fault => Accepted(resp),
                Resp.Code.NeedLogin => Unauthorized(resp),
                _ => throw new ArgumentException("无效的返回数据")
            };

            return result;
        }
    }
}
