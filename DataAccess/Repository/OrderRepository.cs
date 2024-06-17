using BusinessObject.Model;
using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SaleManagerContext context;
        public OrderRepository(SaleManagerContext context)
        {
            this.context = context;
        }
        public void Add(Order order)
        {
            OrderDAO.Instance.Add(order);
        }

        public IEnumerable<Order> FindAllByStartTimeAndEndTime(DateTime start, DateTime end)
        {
            return OrderDAO.Instance.FindAll(order => order.OrderDate >= start && order.OrderDate <= end);
        }

        public IEnumerable<Order> FindByEmail(string email)
        {
            return OrderDAO.Instance.FindAll(order => order.Member.Email == email);
        }

        public Order FindById(int id)
        {
            return OrderDAO.Instance.FindOne(order => order.OrderId == id);
        }

        public IEnumerable<Order> List()
        {
            return OrderDAO.Instance.List();
        }

        public void Remove(Order order)
        {
            OrderDAO.Instance.Delete(order);
        }

        public void Update(Order order)
        {
            OrderDAO.Instance.Update(order);
        }

        public IEnumerable<OrderDetail> GetOrderDetailsByOrderId(int orderId)
        {
            return OrderDAO.Instance.GetOrderDetailsByOrderId(orderId);
        }
        public void AddOrderDetail(OrderDetail orderDetail)
        {
            try
            {
                context.OrderDetails.Add(orderDetail);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public void RemoveOrderDetail(OrderDetail orderDetail)
        {
            OrderDAO.Instance.RemoveOrderDetail(orderDetail);
        }
        IEnumerable<Order> IOrderRepository.FindAllBy(OrderFilter filter)
        {
            if (filter != null)
            {
                return OrderDAO.Instance.FindAll(order =>
                    (filter.StartDate == null || order.OrderDate >= filter.StartDate) &&
                    (filter.EndDate == null || order.OrderDate <= filter.EndDate) ||
                    (filter.StartDate != null && filter.EndDate != null && order.OrderDate >= filter.StartDate && order.OrderDate <= filter.EndDate))
                    .OrderByDescending(order => order.OrderDate).ToList();
            }
            return List();
        }
        public decimal GetOrderTotal(int orderId)
        {
            var orderDetails = GetOrderDetailsByOrderId(orderId);
            return orderDetails.Sum(od => od.UnitPrice * od.Quantity);
        }
    }
}
