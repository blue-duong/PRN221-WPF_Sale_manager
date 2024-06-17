using BusinessObject.Model;
using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IOrderRepository
    {
        IEnumerable<Order> List();
        IEnumerable<Order> FindAllBy(OrderFilter filter);
        IEnumerable<OrderDetail> GetOrderDetailsByOrderId(int orderId);
        void Add(Order order);
        void Update(Order order);
        void Remove(Order order);
        decimal GetOrderTotal(int orderId);
        Order FindById(int id);
        void RemoveOrderDetail(OrderDetail orderDetail);
        void AddOrderDetail(OrderDetail orderDetail);
        IEnumerable<Order> FindByEmail(string email);
        IEnumerable<Order> FindAllByStartTimeAndEndTime(DateTime start,DateTime end);

    }
}
